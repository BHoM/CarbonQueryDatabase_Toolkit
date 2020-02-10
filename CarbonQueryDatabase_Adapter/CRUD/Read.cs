using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BH.Adapter.CarbonQueryDatabase;
using BH.oM.Base;
using BH.oM.Adapter;
using BH.oM.Adapter.CarbonQueryDatabase;
using BH.oM.HTTP;
using BH.Engine.HTTP;
using BH.Engine.CarbonQueryDatabase;
using BH.Adapter;
using BH.Engine.Serialiser;
using BH.Engine.Reflection;
using BH.oM.CarbonQueryDatabase;

namespace BH.Adapter.CarbonQueryDatabase
{
    public partial class CarbonQueryDatabaseAdapter : BHoMAdapter
    {
        /***************************************************/
        /**** Adapter overload method                   ****/
        /***************************************************/
        protected override IEnumerable<IBHoMObject> IRead(Type type, IList ids, ActionConfig actionConfig = null)
        {
            dynamic elems = null;

            int count = 0;
            string name = "";

            CQDConfig config = actionConfig as CQDConfig;
            if (config != null)
            {
                count = config.Count;
                name = config.NameLike;
            }
            
            //Choose what to pull out depending on the type.
            if (type == typeof(BH.oM.Base.BHoMObject))
                elems = ReadEPDData(ids as dynamic, count, name);

            return elems;
        }

        /***************************************************/
        /**** Private specific read methods             ****/
        /***************************************************/

        private List<EPDData> ReadEPDData(List<string> ids = null, int count = 0, string name = "")
        {
            //Add parameters per config
            CustomObject requestParams = new CustomObject();
            if (count != 0)
            {
                requestParams.CustomData.Add("page_size", count);
            }
            if (name != "")
            {
                requestParams.CustomData.Add("name__like", name);
            }

            //Create GET Request
            GetRequest epdGetRequest = BH.Engine.CarbonQueryDatabase.Create.CQDGetRequest("epds", m_bearerToken, requestParams);
            string response = BH.Engine.HTTP.Compute.MakeRequest(epdGetRequest);
            List<object> responseObjs = null;
            if (response == null)
            {
                BH.Engine.Reflection.Compute.RecordWarning("No response received, check bearer token and connection.");
                return null;
            }
            else if (response.StartsWith("{") || response.StartsWith("[")) //Check if the response is a valid json
                responseObjs = new List<object>() { Engine.Serialiser.Convert.FromJson(response) };
            else
            {
                BH.Engine.Reflection.Compute.RecordWarning("Response is not a valid JSON. How'd that happen?");
                return null;
            }

            //Convert nested customObject from serialization to list of epdData objects
            List<EPDData> epdDataFromRequest = new List<EPDData>();
            object epdObjects = Engine.Reflection.Query.PropertyValue(responseObjs[0], "Objects");
            IEnumerable objList = epdObjects as IEnumerable;
            if (objList != null)
            {
                foreach (CustomObject co in objList)
                {
                    EPDData epdData = BH.Engine.CarbonQueryDatabase.Convert.ToBHoMObject(co);
                    epdDataFromRequest.Add(epdData);
                }
            }

            return epdDataFromRequest;
        }
        /***************************************************/
     }
}

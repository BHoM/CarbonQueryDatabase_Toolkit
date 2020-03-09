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
using BH.oM.LifeCycleAnalysis;
using BH.Adapter;
using BH.Engine.Serialiser;
using BH.Engine.Reflection;

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
            CQDConfig config = null;

            if (actionConfig is CQDConfig)
                config = actionConfig as CQDConfig;

            //Choose what to pull out depending on the type.
            if (type == typeof(EnvironmentalProductDeclaration))
                elems = ReadEnvironmentalProductDeclaration(ids as dynamic, config);
            else if (type == typeof(SectorEnvironmentalProductDeclaration))
                elems = ReadSectorEnvironmentalProductDeclaration(ids as dynamic, config);

            return elems;
        }

        /***************************************************/
        /**** Private specific read methods             ****/
        /***************************************************/
        private List<EnvironmentalProductDeclaration> ReadEnvironmentalProductDeclaration(List<string> ids = null, CQDConfig config = null)
        {
            //Add parameters per config
            CustomObject requestParams = new CustomObject();
            int count = 0;
            string name = null;
            string plantName = null;

            if (config != null)
            {
                {
                    count = config.Count;
                    name = config.NameLike;
                    plantName = config.PlantName;
                }
                if (count != 0)
                {
                    requestParams.CustomData.Add("page_size", count);
                }
                if (name != null)
                {
                    requestParams.CustomData.Add("name__like", name);
                }
                if (plantName != null)
                {
                    requestParams.CustomData.Add("plant__name__like", plantName);
                }
            }

            //Create GET Request
            GetRequest epdGetRequest = BH.Engine.CarbonQueryDatabase.Create.CQDGetRequest("epds", m_bearerToken, requestParams);
            string reqString = epdGetRequest.ToUrlString();
            string response = BH.Engine.HTTP.Compute.MakeRequest(epdGetRequest);
            List<object> responseObjs = null;
            if (response == null)
            {
                BH.Engine.Reflection.Compute.RecordWarning("No response received, check bearer token and connection.");
                return null;
            }

            //Check if the response is a valid json
            else if (response.StartsWith("{") || response.StartsWith("["))
                responseObjs = new List<object>() { Engine.Serialiser.Convert.FromJson(response) };

            else
            {
                BH.Engine.Reflection.Compute.RecordWarning("Response is not a valid JSON. How'd that happen?");
                return null;
            }

            //Convert nested customObject from serialization to list of epdData objects
            List<EnvironmentalProductDeclaration> epdDataFromRequest = new List<EnvironmentalProductDeclaration>();

            object epdObjects = Engine.Reflection.Query.PropertyValue(responseObjs[0], "Objects");
            IEnumerable objList = epdObjects as IEnumerable;
            if (objList != null)
            {
                foreach (CustomObject co in objList)
                {
                    EnvironmentalProductDeclaration epdData = BH.Engine.CarbonQueryDatabase.Convert.ToEPD(co);
                    epdDataFromRequest.Add(epdData);
                }
            }

            return epdDataFromRequest;
        }

        /***************************************************/

        private List<SectorEnvironmentalProductDeclaration> ReadSectorEnvironmentalProductDeclaration(List<string> ids = null, CQDConfig config = null)
        {
            //Add parameters per config
            CustomObject requestParams = new CustomObject();
            int count = 0;
            string name = null;
            string plantName = null;

            if (config != null)
            {
                {
                    count = config.Count;
                    name = config.NameLike;
                    plantName = config.PlantName;
                }
                if (count != 0)
                {
                    requestParams.CustomData.Add("page_size", count);
                }
                if (name != null)
                {
                    requestParams.CustomData.Add("name__like", name);
                }
            }

            //Create GET Request
            GetRequest epdGetRequest = BH.Engine.CarbonQueryDatabase.Create.CQDGetRequest("industry_epds", m_bearerToken, requestParams);
            string response = BH.Engine.HTTP.Compute.MakeRequest(epdGetRequest);
            List<object> responseObjs = null;
            if (response == null)
            {
                BH.Engine.Reflection.Compute.RecordWarning("No response received, check bearer token and connection.");
                return null;
            }

            //Check if the response is a valid json
            else if (response.StartsWith("{") || response.StartsWith("["))
                responseObjs = new List<object>() { Engine.Serialiser.Convert.FromJson(response) };

            else
            {
                BH.Engine.Reflection.Compute.RecordWarning("Response is not a valid JSON. How'd that happen?");
                return null;
            }

            //Convert nested customObject from serialization to list of epdData objects
            List<SectorEnvironmentalProductDeclaration> epdDataFromRequest = new List<SectorEnvironmentalProductDeclaration>();

            object epdObjects = Engine.Reflection.Query.PropertyValue(responseObjs[0], "Objects");
            IEnumerable objList = epdObjects as IEnumerable;
            if (objList != null)
            {
                foreach (CustomObject co in objList)
                {
                    SectorEnvironmentalProductDeclaration epdData = BH.Engine.CarbonQueryDatabase.Convert.ToSectorEPD(co);
                    epdDataFromRequest.Add(epdData);
                }
            }

            return epdDataFromRequest;
        }

        /***************************************************/

    }

}

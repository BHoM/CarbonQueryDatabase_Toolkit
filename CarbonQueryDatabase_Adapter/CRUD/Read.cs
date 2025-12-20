/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2026, the respective contributors. All rights reserved.
 *
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 *                                           
 *                                                                              
 * The BHoM is free software: you can redistribute it and/or modify         
 * it under the terms of the GNU Lesser General Public License as published by  
 * the Free Software Foundation, either version 3.0 of the License, or          
 * (at your option) any later version.                                          
 *                                                                              
 * The BHoM is distributed in the hope that it will be useful,              
 * but WITHOUT ANY WARRANTY; without even the implied warranty of               
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
 * GNU Lesser General Public License for more details.                          
 *                                                                            
 * You should have received a copy of the GNU Lesser General Public License     
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BH.Adapter.CarbonQueryDatabase;
using BH.oM.Base;
using BH.oM.Adapter;
using BH.oM.Adapters.CarbonQueryDatabase;
using BH.oM.Adapters.HTTP;
using BH.Engine.Adapters.HTTP;
using BH.Engine.Adapters.CarbonQueryDatabase;
using BH.oM.LifeCycleAssessment;
using BH.oM.LifeCycleAssessment.MaterialFragments;
using BH.Adapter;
using BH.Engine.Serialiser;
using BH.Engine.Base;

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

            if (type == typeof(EnvironmentalProductDeclaration))
                elems = ReadEnvironmentalProductDeclaration(ids as dynamic, config);

            return elems;
        }
        /***************************************************/
        /**** Private specific read methods             ****/
        /***************************************************/
        private List<EnvironmentalProductDeclaration> ReadEnvironmentalProductDeclaration(List<string> ids = null, CQDConfig config = null)
        {
            int count = config.Count;
            string name = config.NameLike;
            string plantName = config.PlantName;
            string id = config.Id;

            //Create GET Request
            GetRequest epdGetRequest;
            if (id == null)
            {
                epdGetRequest = BH.Engine.Adapters.CarbonQueryDatabase.Create.CarbonQueryDatabaseRequest("epds", m_apiToken, config);
            }
            else
            {
                epdGetRequest = BH.Engine.Adapters.CarbonQueryDatabase.Create.CarbonQueryDatabaseRequest("epds/" + id, m_apiToken, config);
            }

            string reqString = epdGetRequest.ToUrlString();
            string response = BH.Engine.Adapters.HTTP.Compute.MakeRequest(epdGetRequest);
            List<object> responseObjs = null;

            if (response == null)
            {
                BH.Engine.Base.Compute.RecordWarning("No response received, API token and connection.");
                return null;
            }
            //Check if the response is a valid json
            else if (response.StartsWith("{"))
            {
                response = "[" + response + "]";
                responseObjs = new List<object>() { Engine.Serialiser.Convert.FromJsonArray(response) };
            }
            else if (response.StartsWith("["))
            {
                responseObjs = new List<object>() { Engine.Serialiser.Convert.FromJsonArray(response) };
            }
            else
            {
                BH.Engine.Base.Compute.RecordWarning("Response is not a valid JSON. How'd that happen?");
                return null;
            }

            //Convert nested customObject from serialization to list of epdData objects
            List<EnvironmentalProductDeclaration> epdDataFromRequest = new List<EnvironmentalProductDeclaration>();
            object epdObjects = responseObjs[0];
            IEnumerable objList = epdObjects as IEnumerable;
            if (objList != null)
            {
                foreach (CustomObject co in objList)
                {
                    EnvironmentalProductDeclaration epdData = Adapter.CarbonQueryDatabase.Convert.ToEnvironmentalProductDeclaration(co, config);
                    epdDataFromRequest.Add(epdData);
                }
            }
            return epdDataFromRequest;
        }

        /***************************************************/
    }
}





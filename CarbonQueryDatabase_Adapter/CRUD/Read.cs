/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2020, the respective contributors. All rights reserved.
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
            string id = null;

            if (config != null)
            {
                id = config.Id;
                count = config.Count;
                name = config.NameLike;
                plantName = config.PlantName;
            }

            //Create GET Request
            GetRequest epdGetRequest;
            if (id == null)
            {
                epdGetRequest = BH.Engine.Adapters.CarbonQueryDatabase.Create.CarbonQueryDatabaseRequest("epds", m_bearerToken);
            }
            else
            {
                epdGetRequest = BH.Engine.Adapters.CarbonQueryDatabase.Create.CarbonQueryDatabaseRequest("epds/" + id, m_bearerToken);
            }

            string reqString = epdGetRequest.ToUrlString();
            string response = BH.Engine.Adapters.HTTP.Compute.MakeRequest(epdGetRequest);
            List<object> responseObjs = null;
            if (response == null)
            {
                BH.Engine.Reflection.Compute.RecordWarning("No response received, check bearer token and connection.");
                return null;
            }

            //Check if the response is a valid json
            else if (response.StartsWith("{"))
            {
                response = "[" + response + "]";
                responseObjs = new List<object>() { Engine.Serialiser.Convert.FromJson(response) };
            }
            else if (response.StartsWith("["))
            {
                responseObjs = new List<object>() { Engine.Serialiser.Convert.FromJson(response) };
            }

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
                    EnvironmentalProductDeclaration epdData = Adapter.CarbonQueryDatabase.Convert.ToEnvironmentalProductDeclaration(co);
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
            string id = null;

            if (config != null)
            {
                id = config.Id;
                count = config.Count;
                name = config.NameLike;
            }

            //Create GET Request
            GetRequest epdGetRequest;
            if (id == null)
            {
                epdGetRequest = BH.Engine.Adapters.CarbonQueryDatabase.Create.CarbonQueryDatabaseRequest("industry_epds", m_bearerToken);
            }
            else
            {
                epdGetRequest = BH.Engine.Adapters.CarbonQueryDatabase.Create.CarbonQueryDatabaseRequest("industry_epds" + id, m_bearerToken);
            }

            string response = BH.Engine.Adapters.HTTP.Compute.MakeRequest(epdGetRequest);
            List<object> responseObjs = null;
            if (response == null)
            {
                BH.Engine.Reflection.Compute.RecordWarning("No response received, check bearer token and connection.");
                return null;
            }

            //Check if the response is a valid json
            else if (response.StartsWith("{"))
            {
                response = "[" + response + "]";
                responseObjs = new List<object>() { Engine.Serialiser.Convert.FromJson(response) };
            }
            else if (response.StartsWith("["))
            {
                responseObjs = new List<object>() { Engine.Serialiser.Convert.FromJson(response) };
            }

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
                    SectorEnvironmentalProductDeclaration epdData = Adapter.CarbonQueryDatabase.Convert.ToSectorEnvironmentalProductDeclaration(co);
                    epdDataFromRequest.Add(epdData);
                }
            }

            return epdDataFromRequest;
        }

        /***************************************************/

    }

}

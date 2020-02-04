/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2019, the respective contributors. All rights reserved.
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
using BH.oM.Adapter;
using BH.oM.Base;
using BH.oM.HTTP;
using BH.Engine.HTTP;
using BH.Engine.CarbonQueryDatabase;
using BH.oM.LifeCycleAnalysis;
using BH.Engine.CarbonQueryDatabase;
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
            //Choose what to pull out depending on the type. Also see example methods below for pulling out bars and dependencies
            if (type == typeof(BH.oM.Base.BHoMObject))
                elems = ReadEPDData(ids as dynamic);

            return elems;
        }


        /***************************************************/

        /***************************************************/
        /**** Private specific read methods             ****/
        /***************************************************/

        //The List<string> in the methods below can be changed to a list of any type of identification more suitable for the toolkit

        private List<EPDData> ReadEPDData(List<string> ids = null)
        {
            GetRequest epdGetRequest = BH.Engine.CarbonQueryDatabase.Create.CQDGetRequest("epds", m_bearerToken); //TODO Add parameters option per config
            string response = BH.Engine.HTTP.Compute.MakeRequest(epdGetRequest);
            List<object> responseObjs = null;
            if (response == null)
            {
                BH.Engine.Reflection.Compute.RecordWarning("No response received, check bearer token and connection.");
                return null;
            }

            // check if the response is a valid json
            else if (response.StartsWith("{") || response.StartsWith("["))
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

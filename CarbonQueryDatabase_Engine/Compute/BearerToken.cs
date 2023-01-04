/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.ComponentModel;
using BH.oM.Base.Attributes;

namespace BH.Engine.Adapters.CarbonQueryDatabase
{
    public static partial class Compute
    {
        [Description("Returns a bearer token for the CarbonQueryDatabase system from the provided username and password")]
        [Input("username", "Your username for the system")]
        [Input("password", "Your password for the system - case sensitive, do not share scripts with this saved")]
        [Output("bearerToken", "The bearer token to use the database system or the full response string if there was an error")]
        public static string CQDBearerToken(string username, string password)
        {
            string apiAddress = "https://etl-api.cqd.io/api/rest-auth/login";
                
                System.Net.ServicePointManager.SecurityProtocol =
                SecurityProtocolType.Ssl3 |
                SecurityProtocolType.Tls12 |
                SecurityProtocolType.Tls11 |
                SecurityProtocolType.Tls;

            HttpClient client = new HttpClient();

            //Add headers per api auth requirements
            client.DefaultRequestHeaders.Add("accept", "application/json");

            //Post login auth request and return token to m_bearerKey
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, apiAddress);

            string loginString = "{\"username\":\"" + username + "\",\"password\":\"" + password + "\"}";
            request.Content = new StringContent(loginString, Encoding.UTF8, "application/json");

            HttpResponseMessage response = client.SendAsync(request).Result;
            if (response.IsSuccessStatusCode)
            {

                string responseAuthString = response.Content.ReadAsStringAsync().Result;
                if (responseAuthString.Split('"').Length >= 3)
                    return responseAuthString.Split('"')[3];
                else
                {
                    BH.Engine.Base.Compute.RecordError("We did not receive the response we expected. The response was '" + responseAuthString + "'");
                    return null;
                }
            }
            else
            {
                BH.Engine.Base.Compute.RecordError("Request failed with code '" + response.StatusCode.ToString() + "'. Please check credentials and try again.");
                return null;
            }
        }
    }
}




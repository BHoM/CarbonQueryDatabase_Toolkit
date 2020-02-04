using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Base;
using BH.Adapter;
using System.Net;
using System.Net.Http;
using System.ComponentModel;
using BH.oM.Reflection.Attributes;

namespace BH.Adapter.CarbonQueryDatabase
{
    public partial class CarbonQueryDatabaseAdapter : BHoMAdapter
    {
        /***************************************************/
        /**** Public fields                             ****/
        /***************************************************/

        public static string AdapterID = "CarbonQueryDatabaseAdapter";

        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        [Description("Description goes here. Description goes here.")]
        [Input("username", "Provide EC3 Username")]
        [Input("password", "Provide EC3 Password")]
        [Output("adapter", "adapter results")]

        public CarbonQueryDatabaseAdapter(string username = "", string password = "", bool active = false)
        {
            if (active)
            {
                System.Net.ServicePointManager.SecurityProtocol =
                  SecurityProtocolType.Ssl3 |
                  SecurityProtocolType.Tls12 |
                  SecurityProtocolType.Tls11 |
                  SecurityProtocolType.Tls;

                HttpClient client = new HttpClient();

                //Add headers per api auth requirements
                client.DefaultRequestHeaders.Add("accept", "application/json");

                //Post login auth request and return token to m_bearerKey
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, m_apiAddress);

                string loginString = "{\"username\":\"" + username + "\",\"password\":\"" + password + "\"}";
                request.Content = new StringContent(loginString, Encoding.UTF8, "application/json");

                HttpResponseMessage response = client.SendAsync(request).Result;

                string responseAuthString = response.Content.ReadAsStringAsync().Result;

                m_bearerToken = "";

                if (responseAuthString.Split('"').Length >= 3)
                    m_bearerToken = responseAuthString.Split('"')[3];
                else
                {
                    BH.Engine.Reflection.Compute.RecordError("The response was not a valid token. Please check credentials and try again.");
                }
            }

        }

        /***************************************************/
        /*** Private Fields                              ***/
        /***************************************************/

        private static string m_apiAddress = "https://etl-api.cqd.io/api/rest-auth/login";
        private static string m_bearerToken;
    }
}
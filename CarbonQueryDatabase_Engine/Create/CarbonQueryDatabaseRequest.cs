using BH.oM.Base;
using System.Collections.Generic;
using BH.oM.HTTP;
using BH.oM.Reflection.Attributes;
using System.ComponentModel;

namespace BH.Engine.CarbonQueryDatabase
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public  Method                            ****/
        /***************************************************/

        /***************************************************/

        [Description("Create a GetRequest for the CarbonQueryDatabase (api-ui.cqd.io).")]
        [Input("apiCommand", "apiCommand to create GetRequest with")]
        [Input("bearerToken", "CarbonQueryDatabase bearer token")]
        [Input("parameters", "parameters to create GetRequest with")]
        [Output("GetRequest", "GetRequest with formatting per CarbonQueryDatabase api.")]
        public static GetRequest CQDGetRequest(string apiCommand, string bearerToken, CustomObject parameters = null)
        {
            return new BH.oM.HTTP.GetRequest
            {
                BaseUrl = "https://etl-api.cqd.io/api/" + apiCommand,
                Headers = new Dictionary<string, object>()
                {
                    { "Authorization", "Bearer " +  bearerToken }
                },
                Parameters = parameters?.CustomData
            };
        }

        /***************************************************/
    }
}

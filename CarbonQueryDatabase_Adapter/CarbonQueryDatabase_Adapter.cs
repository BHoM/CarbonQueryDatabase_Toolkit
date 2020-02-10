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
using BH.Engine.CarbonQueryDatabase;

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

        [Description("Adapter to connect to CarbonQueryDatabase.")]
        [Input("username", "Provide EC3 Username")]
        [Input("password", "Provide EC3 Password")]
        [Output("adapter", "Adapter results")]

        public CarbonQueryDatabaseAdapter(string username = "", string password = "", bool active = false)
        {
            if (active)
            {
                m_bearerToken = Compute.CQDBearerToken(username, password);
            }
        }

        /***************************************************/
        /*** Private Fields                              ***/
        /***************************************************/

        private static string m_bearerToken = null;

    }
}
using System.Collections.Generic;
using BH.oM.Base;

namespace BH.oM.CarbonQueryDatabase
{
    public class EPDData : BHoMObject
    {
        /***************************************************/
        /**** Properties                                ****/
        /***************************************************/

        public string id { get; set; } = "";

        public string name { get; set; } = "";

        public string manufacturer { get; set; } = "";

        public string plant { get; set; } = "";

        public int postalCode { get; set; } = 0;

        public string density { get; set; } = "";

        public string gwpPerKG { get; set; } = "";

        /***************************************************/
    }
}

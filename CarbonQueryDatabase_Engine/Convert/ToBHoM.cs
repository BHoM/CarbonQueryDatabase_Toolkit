using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.ComponentModel;
using BH.oM.Reflection.Attributes;
using BH.oM.Base;
using BH.oM.LifeCycleAnalysis;
using BH.Engine.Reflection;

namespace BH.Engine.CarbonQueryDatabase
{
    public static partial class Convert
    {
        /***************************************************/
        /****           Public Methods                  ****/
        /***************************************************/

        [Description("Convert a CustomObject with EPD CustomData into an EPDData object")]
        [Input("customObj", "A CustomObject containing EPD Data in its CustomData")]
        [Output("EPDData", "An EPDData object")]
        public static EPDData ToBHoMObject(this CustomObject obj)
        {
            int result = 0;

            EPDData data = new EPDData
            {
                id = obj.PropertyValue("id")?.ToString() ?? "",
                name = obj.PropertyValue("name")?.ToString() ?? "",
                manufacturer = obj.PropertyValue("manufacturer.name")?.ToString() ?? "",
                plant = obj.PropertyValue("plant.name")?.ToString() ?? "",
                postalCode = int.TryParse(obj.PropertyValue("plant.postal_code")?.ToString() ?? "", out result) ? result : 0,
                density = obj.PropertyValue("density")?.ToString() ?? "",
                gwpPerKG = obj.PropertyValue("gwp_per_kg")?.ToString() ?? "",                
            };
                                
            return data;
        }

         /***************************************************/
    }
}

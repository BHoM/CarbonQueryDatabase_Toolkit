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
using System.Collections;

namespace BH.Engine.CarbonQueryDatabase
{
    public static partial class Convert
    {
        /***************************************************/
        /****           Public Methods                  ****/
        /***************************************************/

        public static ProductEPD ToProductEPD(this CustomObject obj)
        {
            int result = 0;

            IEnumerable<string> standards = null;
            if (obj.PropertyValue("industry_standards") != null)
                standards = obj.PropertyValue("industry_standards") as IEnumerable<string>;

            ProductEPD epd = new ProductEPD
            {
                Id = obj.PropertyValue("id")?.ToString() ?? "",
                Name = obj.PropertyValue("name")?.ToString() ?? "",
                Manufacturer = obj.PropertyValue("manufacturer.name")?.ToString() ?? "",
                Plant = obj.PropertyValue("plant.name")?.ToString() ?? "",
                PostalCode = int.TryParse(obj.PropertyValue("plant.postal_code")?.ToString() ?? "", out result) ? result : 0,
                Density = obj.PropertyValue("density")?.ToString() ?? "",
                GwpPerKG = obj.PropertyValue("gwp_per_kg")?.ToString() ?? "",
                BiogenicEmbodiedCarbon = obj.PropertyValue("biogenic_embodied_carbon_z") != null ? System.Convert.ToDouble(obj.PropertyValue("biogenic_embodied_carbon_z")) : double.NaN,
                DeclaredUnit = obj.PropertyValue("declared_unit")?.ToString() ?? "",
                Description = obj.PropertyValue("description")?.ToString() ?? "",
                IndustryStandards = standards != null  ? standards.ToList() : new List<string>(),
            };
                                
            return epd;
        }

        /***************************************************/

        public static SectorEPD ToSectorEPD(this CustomObject obj)
        {
            List<string> publisherNames = new List<string>();

            if (obj.PropertyValue("publishers") != null)
            {
                IEnumerable pubs = (IEnumerable)obj.PropertyValue("publishers");
                foreach (CustomObject pub in pubs)
                {
                    publisherNames.Add(pub.PropertyValue("name").ToString());
                }
            }

            List<string> jurisdictionNames = new List<string>();
            if (obj.PropertyValue("geography") != null)
            {
                {
                    IEnumerable jurs = (IEnumerable)obj.PropertyValue("geography.country_codes");
                    foreach (object jur in jurs)
                    {
                        jurisdictionNames.Add(jur.ToString());
                    }
                }
            }
                

            SectorEPD epd = new SectorEPD
            {
                Id = obj.PropertyValue("id")?.ToString() ?? "",
                Name = obj.PropertyValue("name")?.ToString() ?? "",
                Density = obj.PropertyValue("density")?.ToString() ?? "",
                GwpPerDeclaredUnit = obj.PropertyValue("gwp")?.ToString() ?? "",
                BiogenicEmbodiedCarbon = obj.PropertyValue("biogenic_embodied_carbon_z") != null ? System.Convert.ToDouble(obj.PropertyValue("biogenic_embodied_carbon_z")) : double.NaN,
                DeclaredUnit = obj.PropertyValue("declared_unit")?.ToString() ?? "",
                Description = obj.PropertyValue("description")?.ToString() ?? "",
                Jurisdiction = jurisdictionNames,
                Publisher = publisherNames,
            };

            return epd;
        }

        /***************************************************/
    }
}

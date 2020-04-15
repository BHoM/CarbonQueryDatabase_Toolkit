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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.ComponentModel;
using BH.oM.Reflection.Attributes;
using BH.oM.Base;
using BH.oM.LifeCycleAssessment;
using BH.Engine.Reflection;
using BH.Engine.Localisation;
using System.Collections;

namespace BH.Adapter.CarbonQueryDatabase
{
    public static partial class Convert
    {
        /***************************************************/
        /****           Public Methods                  ****/
        /***************************************************/

        public static EnvironmentalProductDeclaration ToEnvironmentalProductDeclaration(this CustomObject obj)
        {
            int result = 0;

            IEnumerable<string> standards = null;
            if (obj.PropertyValue("industry_standards") != null)
                standards = obj.PropertyValue("industry_standards") as IEnumerable<string>;

            string densityString = obj.PropertyValue("density")?.ToString() ?? "";
            double densityVal = GetValFromString(densityString);
            string densityUnits = GetUnitsFromString(densityString);
            double density = ConvertToSI(densityVal, densityUnits);
            string gwp = obj.PropertyValue("gwp")?.ToString() ?? "";
            double gwpVal = System.Convert.ToDouble(gwp.Substring(0, gwp.IndexOf(" ")));

            string declaredUnit = obj.PropertyValue("declared_unit")?.ToString() ?? "";
            string epdUnit = GetUnitsFromString(declaredUnit);
            double epdUnitMult = ConvertToSI(1, epdUnit);

            EnvironmentalProductDeclaration epd = new EnvironmentalProductDeclaration
            {
                QuantityType = QuantityType.Volume,
                Id = obj.PropertyValue("id")?.ToString() ?? "",
                Name = obj.PropertyValue("name")?.ToString() ?? "",
                Manufacturer = obj.PropertyValue("manufacturer.name")?.ToString() ?? "",
                Plant = obj.PropertyValue("plant.name")?.ToString() ?? "",
                PostalCode = int.TryParse(obj.PropertyValue("plant.postal_code")?.ToString() ?? "", out result) ? result : 0,
                Density = density,
                GlobalWarmingPotential = gwpVal,
                BiogenicEmbodiedCarbon = obj.PropertyValue("biogenic_embodied_carbon") != null ? System.Convert.ToDouble(obj.PropertyValue("biogenic_embodied_carbon_z")) : double.NaN,
                Description = obj.PropertyValue("description")?.ToString() ?? "",
                IndustryStandards = standards != null  ? standards.ToList() : new List<string>(),
            };
                                
            return epd;
        }

        /***************************************************/

        public static SectorEnvironmentalProductDeclaration ToSectorEnvironmentalProductDeclaration(this CustomObject obj)
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
                IEnumerable jurs = (IEnumerable)obj.PropertyValue("geography.country_codes");
                foreach (object jur in jurs)
                {
                   jurisdictionNames.Add(jur.ToString());
                }
            }
            string densityString = obj.PropertyValue("density_max")?.ToString() ?? "";
            double densityVal = GetValFromString(densityString);
            string densityUnits = GetUnitsFromString(densityString);
            double density = ConvertToSI(densityVal, densityUnits);
            string gwp = obj.PropertyValue("gwp")?.ToString() ?? "";
            double gwpVal = System.Convert.ToDouble(gwp.Substring(0, gwp.IndexOf(" ")));

            string declaredUnit = obj.PropertyValue("declared_unit")?.ToString() ?? "";
            string epdUnit = GetUnitsFromString(declaredUnit);
            double epdUnitMult = ConvertToSI(1, epdUnit);

            SectorEnvironmentalProductDeclaration epd = new SectorEnvironmentalProductDeclaration
            {
                QuantityType = QuantityType.Volume,
                Id = obj.PropertyValue("id")?.ToString() ?? "",
                Name = obj.PropertyValue("name")?.ToString() ?? "",
                Density = density,
                GlobalWarmingPotential = gwpVal,
                BiogenicEmbodiedCarbon = obj.PropertyValue("biogenic_embodied_carbon") != null ? System.Convert.ToDouble(obj.PropertyValue("biogenic_embodied_carbon_z")) : double.NaN,
                Description = obj.PropertyValue("description")?.ToString() ?? "",
                Jurisdiction = jurisdictionNames,
                Publisher = publisherNames,
            };

            return epd;
        }

        /***************************************************/

        public static double ConvertToSI(double val, string unitFrom)
        {
            double unitMult = 1;     
            switch(unitFrom)
            {
                case "kg/m3":
                case "kg/m³":
                    unitMult = 1;
                    break;
                case "lb/y3":
                    unitMult = 0.593276;
                    break;
                case "yd3":
                case "y3":
                case "yd³":
                case "y³":
                    unitMult = 1.30795;
                    break;
                case "t":
                case "short ton":
                    unitMult = 0.000984207;
                    break;
                case "tonne":
                case "metric ton":
                    unitMult = 0.001;
                    break;
                case "sq ft":
                case "ft2":
                case "square ft":
                case "SF":
                    unitMult = 10.7639;
                    break;
                case "in":
                case "inches":
                    return Engine.Localisation.Length.Convert.FromInch(val);
                case "ft":
                case "feet":
                    return Engine.Localisation.Length.Convert.FromFoot(val);
            }
            double valueSI = unitMult * val;
            return valueSI;
        }

        /***************************************************/

        public static double GetValFromString(this string str)
        {
            string[] strings = str.Split(' ');
            string numString = strings[0];
            double val = double.NaN;
            Double.TryParse(numString, out val);
            return val;
        }

        /***************************************************/

        public static string GetUnitsFromString(this string str)
        {
            string[] strings = str.Split(' ');
            string units = (strings.Length > 1) ? strings[1] : "";
            return units;
        }

    }
}

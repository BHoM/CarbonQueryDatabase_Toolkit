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
using System.Text.RegularExpressions;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.ComponentModel;
using BH.oM.Reflection.Attributes;
using BH.oM.Base;
using BH.oM.LifeCycleAssessment;
using BH.oM.LifeCycleAssessment.MaterialFragments;
using BH.Engine.Reflection;
using System.Collections;
using BH.Engine.Adapters.CarbonQueryDatabase;
using BH.Engine.Units;

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

            string declaredUnit = obj.PropertyValue("declared_unit")?.ToString() ?? "";
            string epdUnit = GetUnitsFromString(declaredUnit);
            double declaredVal = GetValFromString(declaredUnit);
            QuantityType quantityType = GetQuantityTypeFromString(epdUnit);
            double epdUnitMult = ConvertToSI(1/declaredVal, epdUnit);

            string densityString = obj.PropertyValue("density")?.ToString() ?? "";
            double densityVal = GetValFromString(densityString);
            string densityUnits = GetUnitsFromString(densityString);
            double density = ConvertToSI(densityVal, densityUnits);
            string gwp = obj.PropertyValue("gwp")?.ToString() ?? "";
            double gwpVal = (gwp == "") ? double.NaN : System.Convert.ToDouble(gwp.Substring(0, gwp.IndexOf(" "))) * epdUnitMult;

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
                BiogenicEmbodiedCarbon = obj.PropertyValue("biogenic_embodied_carbon") != null ? System.Convert.ToDouble(obj.PropertyValue("biogenic_embodied_carbon_z")) * epdUnitMult : double.NaN,
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

            string declaredUnit = obj.PropertyValue("declared_unit")?.ToString() ?? "";
            string epdUnit = GetUnitsFromString(declaredUnit);
            double declaredVal = GetValFromString(declaredUnit);
            QuantityType quantityType = GetQuantityTypeFromString(epdUnit);
            double epdUnitMult = ConvertToSI(1/declaredVal, epdUnit);

            string densityString = obj.PropertyValue("density_max")?.ToString() ?? "";
            double densityVal = GetValFromString(densityString);
            string densityUnits = GetUnitsFromString(densityString);
            double density = ConvertToSI(densityVal, densityUnits);
            string gwp = obj.PropertyValue("gwp")?.ToString() ?? "";
            double gwpVal = (gwp == "") ? double.NaN : System.Convert.ToDouble(gwp.Substring(0, gwp.IndexOf(" "))) * epdUnitMult;

            SectorEnvironmentalProductDeclaration epd = new SectorEnvironmentalProductDeclaration
            {
                QuantityType = quantityType,
                Id = obj.PropertyValue("id")?.ToString() ?? "",
                Name = obj.PropertyValue("name")?.ToString() ?? "",
                Density = density,
                GlobalWarmingPotential = gwpVal,
                BiogenicEmbodiedCarbon = obj.PropertyValue("biogenic_embodied_carbon") != null ? System.Convert.ToDouble(obj.PropertyValue("biogenic_embodied_carbon_z")) * epdUnitMult : double.NaN,
                Description = obj.PropertyValue("description")?.ToString() ?? "",
                Jurisdiction = jurisdictionNames,
                Publisher = publisherNames,
            };

            return epd;
        }

        /***************************************************/



        public static QuantityType GetQuantityTypeFromString(string unitFrom)
        {    
            switch(unitFrom)
            {
                case "yd3":
                case "y3":
                case "yd³":
                case "y³":
                case "m3":
                case "m³":
                    return QuantityType.Volume;
                case "t":
                case "short ton":
                case "tonne":
                case "metric ton":
                case "lb":
                case "kg":
                    return QuantityType.Mass;
                case "sq ft":
                case "sqft":
                case "ft2":
                case "square ft":
                case "SF":
                case "m2":
                case "M2":
                case "sq m":
                case "m²":
                    return QuantityType.Area;
                case "in":
                case "inches":
                case "ft":
                case "feet":
                case "m":
                case "meters":
                case "metres":
                    return QuantityType.Length;
            }
            return QuantityType.Undefined;
        }

        /***************************************************/

        public static double ConvertToSI(double val, string unitTo)
        {
            double unitMult = 1;
            switch (unitTo)
            {
                case "kg/m3":
                case "kg/m³":
                    unitMult = 1;
                    break;
                case "lb/y3":
                    unitMult = 1.68555;
                    break;
                case "yd3":
                case "y3":
                case "yd³":
                case "y³":
                    return val.ToCubicYard();
                case "short ton":
                    unitMult = 0.00110231;
                    break;
                case "tonne":
                case "t":
                case "metric ton":
                    unitMult = 0.001;
                    break;
                case "lb":
                case "pound":
                    return val.ToPoundForce();
                case "sq ft":
                case "sqft":
                case "ft2":
                case "square ft":
                case "SF":
                    unitMult = 10.7639;
                    break;
                case "in":
                case "inches":
                    return val.ToInch();
                case "ft":
                case "feet":
                    return val.ToFoot();
            }
            double valueSI = unitMult * val;
            if (valueSI == 0)
            { valueSI = double.NaN; }
            return valueSI;
        }

        /***************************************************/

        public static double GetValFromString(this string str)
        {
            Match match = Regex.Match(str, "[A-Za-z\\s]");
            string numString = str.Substring(0, match.Index);
            double val = double.NaN;
            Double.TryParse(numString, out val);
            if (numString == "")
                { val = 1; }
            return val;
        }

        /***************************************************/

        public static string GetUnitsFromString(this string str)
        {
            Match match = Regex.Match(str, "[A-Za-z]");
            string units = str.Substring(match.Index);
            return units;
        }

    }
}

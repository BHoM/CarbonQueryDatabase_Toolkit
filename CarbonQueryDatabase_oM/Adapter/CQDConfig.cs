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

using BH.oM.Adapter;
using System.Collections.Generic;
using System.ComponentModel;

namespace BH.oM.Adapter.CarbonQueryDatabase
{
    [Description("This Config can be specified in the `ActionConfig` input of any Adapter Action (e.g. Push).")]
    // Note: this will get passed within any CRUD method (see their signature). 
    // In order to access its properties, you will need to cast it to `CarbonQueryDatabaseActionConfig`.
    public class CQDConfig : ActionConfig
    {
        /***************************************************/
        /**** Public Properties                         ****/
        /***************************************************/

        [Description("Specifies ID to search and return objects for in CarbonQueryDatabase. If this is specified it supersedes other input parameters.")]
        public virtual string Id { get; set; } = null;

        [Description("Sets maximum amount of items to return from CarbonQueryDatabase")]
        public virtual int Count { get; set; } = 0;

        [Description("Specifies string to search and return objects for in CarbonQueryDatabase, ie RedBuilt RedLam LVL")]
        public virtual string NameLike { get; set; } = null;

        [Description("Specifies plant name to search and return objects for in CarbonQueryDatabase, ie Dupont")]
        public virtual string PlantName { get; set; } = null;

        /***************************************************/
    }
}

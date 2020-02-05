using BH.oM.Adapter;
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

        [Description("Sets maximum amount of items to return from CarbonQueryDatabase")]
        public int Count { get; set; } = 0;

        [Description("Specifies string to search and return objects for in CarbonQueryDatabase")]
        public string NameLike { get; set; } = null;

        /***************************************************/
    }
}

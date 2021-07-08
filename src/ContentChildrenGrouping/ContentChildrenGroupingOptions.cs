using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPiServer.ServiceLocation;

namespace ContentChildrenGrouping
{
    [Options]
    public class ContentChildrenGroupingOptions
    {
        /// <summary>
        /// When true, then routing is globally enabled, default true
        /// </summary>
        public bool RouterEnabled { get; set; } = true;

        /// <summary>
        /// When true, thenduring content saving the structure is updated, default true
        /// </summary>
        public bool StructureUpdateEnabled { get; set; } = false;
    }
}

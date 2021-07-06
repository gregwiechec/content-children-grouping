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
        /// When true then routing is globally enabled
        /// </summary>
        public bool RouterEnabled { get; set; } = true;
    }
}

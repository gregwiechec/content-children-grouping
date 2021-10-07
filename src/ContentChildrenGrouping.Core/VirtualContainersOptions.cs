using EPiServer.ServiceLocation;

namespace ContentChildrenGrouping.Core
{
    [Options]
    public class VirtualContainersOptions
    {
        public bool Enabled { get; set; } = false;
    }
}
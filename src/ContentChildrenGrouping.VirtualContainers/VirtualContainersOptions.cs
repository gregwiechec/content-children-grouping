using EPiServer.ServiceLocation;

namespace ContentChildrenGrouping.VirtualContainers
{
    [Options]
    public class VirtualContainersOptions
    {
        public bool Enabled { get; set; } = false;
    }
}
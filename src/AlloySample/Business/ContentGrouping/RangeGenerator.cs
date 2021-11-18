using ContentChildrenGrouping.Core.ContainerNameGenerator;
using ContentChildrenGrouping.VirtualContainers.ContainerNameGenerator;
using EPiServer.Core;

namespace AlloySample.Business.ContentGrouping
{
    public class RangeGenerator: IGroupNameGenerator
    {
        public string GetName(IContent content)
        {
            if (content.Name.Length == 0)
            {
                return "!default";
            }

            var name = content.Name.ToLower()[0];
            if (name <= 'g')
            {
                return "A-F";
            }

            if (name >= 'h' && name <= 'n')
            {
                return "H-N";
            }

            if (name >= 'm' && name <= 's')
            {
                return "M-S";
            }

            return "T-Z";
        }
    }
}
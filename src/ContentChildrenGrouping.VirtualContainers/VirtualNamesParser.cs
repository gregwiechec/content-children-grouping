using System;
using System.Collections.Generic;
using System.Linq;
using ContentChildrenGrouping.Core.ContainerNameGenerator;
using EPiServer.Core;

namespace ContentChildrenGrouping.VirtualContainers
{
    /// <summary>
    /// create VirtualContainer content link and parse parameters from ContentLink
    /// </summary>
    public static class VirtualNamesParser
    {
        public const string ProviderPrefix = "VirtualContainers";

        public static bool IsVirtualContainer(this ContentReference contentLink)
        {
            return contentLink.ProviderName?.StartsWith(ProviderPrefix) == true;
        }

        public static bool IsVirtualContainer(this IContent content)
        {
            return IsVirtualContainer(content.ContentLink);
        }

        /// <summary>
        /// Create ContentLink for virtual container
        /// </summary>
        /// <param name="parentContentLink"></param>
        /// <param name="generatedName"></param>
        /// <param name="generatorLevel"></param>
        /// <returns></returns>
        public static ContentReference GetVirtualContentLink(ContentReference parentContentLink, IContent content, List<IGroupNameGenerator> generators, int generatorLevel)
        {
            var names = new List<string>();
            for (var i = 0; i <= generatorLevel; i++)
            {
                var name = generators[i].GetName(content) ?? "";
                // ContentReference provider name cannot contains "_", because it's a separator
                name = name.ToLower().Replace("_", "*");
                names.Add(name);
            }
            var namesStr = string.Join(",", names);
            return new ContentReference(parentContentLink.ID, 0, ProviderPrefix + "-" + namesStr);
        }

        /// <summary>
        /// Get generated names from ContentReference
        /// </summary>
        /// <param name="virtualContainerReference"></param>
        /// <returns></returns>
        public static IEnumerable<string> ParseGeneratorValues(ContentReference virtualContainerReference)
        {
            if (string.IsNullOrWhiteSpace(virtualContainerReference.ProviderName))
            {
                return Enumerable.Empty<string>();
            }

            var names = virtualContainerReference.ProviderName.Substring((ProviderPrefix + "-").Length);
            var result = names.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            return result;
        }

    }
}
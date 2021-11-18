using System;

namespace ContentChildrenGrouping.VirtualContainers.Extensions
{
    internal static class StringExtensions
    {
        public static bool CompareStrings(this string value1, string value)
        {
            return string.Compare(value1, value, StringComparison.InvariantCultureIgnoreCase) == 0;
        }
    }
}
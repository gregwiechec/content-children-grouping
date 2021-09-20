using System;

namespace ContentChildrenGrouping.Extensions
{
    internal static class TypeExtensions
    {
        public static string TypeToString(this Type type)
        {
            if (type == null)
            {
                return null;
            }

            return $"{type.FullName}, {type.Assembly.GetName().Name}";
        }
    }
}
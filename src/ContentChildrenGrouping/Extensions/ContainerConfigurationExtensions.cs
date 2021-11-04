using System;
using ContentChildrenGrouping.Core;
using ContentChildrenGrouping.PhysicalContainers.ContainerModel;

namespace ContentChildrenGrouping.Extensions
{
    internal static class ContainerConfigurationExtensions
    {
        /// <summary>
        /// Get Container type of when null then return default container type
        /// </summary>
        /// <param name="containerConfiguration"></param>
        /// <returns></returns>
        public static Type GetContainerType(this ContainerConfiguration containerConfiguration)
        {
            return containerConfiguration.ContainerType ?? typeof(GroupingContainerPage);
        }
    }
}
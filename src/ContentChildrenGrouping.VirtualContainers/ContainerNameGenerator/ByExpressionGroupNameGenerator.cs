using System;
using EPiServer.Core;
using EPiServer.ServiceLocation;

namespace ContentChildrenGrouping.VirtualContainers.ContainerNameGenerator
{
    [ServiceConfiguration(typeof(IGroupNameGenerator))]
    internal class ByExpressionGroupNameGenerator : IGroupNameGenerator
    {
        private readonly Func<IContent, string> _expression;

        public ByExpressionGroupNameGenerator()
        {
            _expression = content => content.Name;
        }

        public ByExpressionGroupNameGenerator(Func<IContent, string> expression)
        {
            _expression = expression;
        }

        public string GetName(IContent content)
        {
            return _expression(content);
        }
    }
}
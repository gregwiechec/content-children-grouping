using System.Collections.Generic;
using EPiServer.Cms.Shell.UI.Rest.Models.Transforms;
using EPiServer.Cms.Shell.UI.Rest.Models.Transforms.Internal;
using EPiServer.Core;

namespace ContentChildrenGrouping.VirtualContainers
{
    public class CustomHasChildrenEvaluator : IHasChildrenEvaluator
    {
        private readonly IHasChildrenEvaluator _childrenEvaluator;

        public CustomHasChildrenEvaluator(IHasChildrenEvaluator childrenEvaluator)
        {
            _childrenEvaluator = childrenEvaluator;
        }

        public bool CanExecute(IContent content, IModelTransformContext context) =>
            _childrenEvaluator.CanExecute(content, context);

        public int Rank => _childrenEvaluator.Rank;
        public bool Execute(IContent content, IEnumerable<string> typeIdentifiers, bool allLanguages, IModelTransformContext context)
        {
            if (content != null && content.IsVirtualContainer())
            {
                return true;
            }

            return _childrenEvaluator.Execute(content, typeIdentifiers, allLanguages, context);
        }
    }
}
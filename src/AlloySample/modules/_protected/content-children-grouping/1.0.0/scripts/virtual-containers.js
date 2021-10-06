define([
	"epi/shell/dnd/tree/multiDndSource",
	"epi-cms/component/ContentNavigationTree",
	"epi-cms/widget/ContentTree"
], function (
	multiDndSource,
	ContentNavigationTree,
	ContentTree
) {
    return function(configuredContainerIds) {
		var virtaulContainersProvider = "_VirtualContainers";
		
		var originalOnMouseDown = multiDndSource.prototype.onMouseDown;
        multiDndSource.prototype.onMouseDown = function (e) {
			if (this.current && this.current.item.contentLink.indexOf(virtaulContainersProvider) !== -1) {
                return;
            }

			return originalOnMouseDown.apply(this, arguments);
        };
        multiDndSource.prototype.onMouseDown.nom = "onMouseDown";

		var originalOnTreeNodeClicked = ContentNavigationTree.prototype._onTreeNodeClicked;
        ContentNavigationTree.prototype._onTreeNodeClicked = function (page) {
			if (page && page.contentLink.indexOf(virtaulContainersProvider) !== -1) {
                return;
            }

			return originalOnTreeNodeClicked.apply(this, arguments);
        };
        ContentNavigationTree.prototype._onTreeNodeClicked.nom = "_onTreeNodeClicked";

		var originalBuildNodeFromTemplate = ContentTree.prototype.buildNodeFromTemplate;
		ContentTree.prototype.buildNodeFromTemplate = function (args) {
			if (args.item && args.item.contentLink && args.item.contentLink.indexOf(virtaulContainersProvider) !== -1) {
				args.hasContextMenu = false;
            }

			return originalBuildNodeFromTemplate.apply(this, arguments);
        };
		ContentTree.prototype.buildNodeFromTemplate.nom = "originalBuildNodeFromTemplate";
	}
});



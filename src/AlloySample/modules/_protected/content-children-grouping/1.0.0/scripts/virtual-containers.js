define([
	"dojo/Deferred",
	"epi/shell/dnd/tree/multiDndSource",
	"epi-cms/component/ContentNavigationTree",
	"epi-cms/widget/ContentTree"
], function (
	Deferred,
	multiDndSource,
	ContentNavigationTree,
	ContentTree
) {
    return function(configuredContainerIds) {
		var virtaulContainersProvider = "_VirtualContainers";

		// do not allow to select virtual containers
		var originalOnMouseDown = multiDndSource.prototype.onMouseDown;
        multiDndSource.prototype.onMouseDown = function (e) {
			if (this.current && this.current.item.contentLink.indexOf(virtaulContainersProvider) !== -1) {
                return;
            }

			return originalOnMouseDown.apply(this, arguments);
        };
        multiDndSource.prototype.onMouseDown.nom = "onMouseDown";

		// do not allow to click on virtual container
		var originalOnTreeNodeClicked = ContentNavigationTree.prototype._onTreeNodeClicked;
        ContentNavigationTree.prototype._onTreeNodeClicked = function (page) {
			if (page && page.contentLink.indexOf(virtaulContainersProvider) !== -1) {
                return;
            }

			return originalOnTreeNodeClicked.apply(this, arguments);
        };
        ContentNavigationTree.prototype._onTreeNodeClicked.nom = "_onTreeNodeClicked";

		// hide context menu for virtual container
		var originalBuildNodeFromTemplate = ContentTree.prototype.buildNodeFromTemplate;
		ContentTree.prototype.buildNodeFromTemplate = function (args) {
			if (args.item && args.item.contentLink && args.item.contentLink.indexOf(virtaulContainersProvider) !== -1) {
				args.hasContextMenu = false;
            }

			return originalBuildNodeFromTemplate.apply(this, arguments);
        };
		ContentTree.prototype.buildNodeFromTemplate.nom = "buildNodeFromTemplate";

		// update selected path and add virtual containers
		var originalGetTreePath = ContentTree.prototype._getTargetPath;
		ContentTree.prototype._getTargetPath = function (args) {
			if (!this.selectedNode) {
				return originalGetTreePath.apply(this, arguments);
            }
			var parent = this.selectedNode.getParent();
			if (!parent || !parent.item || !parent.item.contentLink) {
				return originalGetTreePath.apply(this, arguments);
			}

			// not a virtual container
			if (parent.item.contentLink.indexOf(virtaulContainersProvider) === -1) {
				return originalGetTreePath.apply(this, arguments);
            }

			var result = originalGetTreePath.apply(this, arguments);
			var deferred = new Deferred();
			result.then(function (targetPath) {
				targetPath.splice(targetPath.length - 1, 0, parent.item.contentLink);
				deferred.resolve(targetPath);
			});
			return deferred.promise;
        };
		ContentTree.prototype._getTargetPath.nom = "_getTargetPath";
	}
});



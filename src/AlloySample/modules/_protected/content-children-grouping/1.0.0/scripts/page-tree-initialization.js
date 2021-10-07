define([
	"epi-cms/component/PageNavigationTree",
	"epi-cms/widget/ContentTree"
], function (
    PageNavigationTree,
	ContentTree
) {
    return function (configurationContainerLinks) {

        // override default page tree icon
        var originalGetIconClass = PageNavigationTree.prototype.getIconClass;
        PageNavigationTree.prototype.getIconClass = function (item, opened) {
            if (configurationContainerLinks.indexOf(item.contentLink) !== -1) {
                return "epi-iconObjectSharedBlockContextual";
            }
            return originalGetIconClass.apply(this, arguments);
        };
        PageNavigationTree.prototype.getIconClass.nom = "getIconClass";

        // override default content tree tooltip
        var originalGetTooltip = ContentTree.prototype.getTooltip;
        ContentTree.prototype.getTooltip = function (item, opened) {
            var result = originalGetTooltip.apply(this, arguments);
            if (configurationContainerLinks.indexOf(item.contentLink) !== -1) {
                return result +
                    "\r\nThis content is configured as container. Structe under this content will be modified automatically.";
            }
            return result;
        };
        ContentTree.prototype.getTooltip.nom = "getIconClass";
    };
});



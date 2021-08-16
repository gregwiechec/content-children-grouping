define([
	"epi-cms/plugin-area/navigation-tree",
    "epi-cms/plugin-area/assets-pane",
    "./search-command"
], function (
    navigationTreePluginArea,
    assetsPanePluginArea,
    SearchCommand
) {
    return function(configurationContainerLinks) {
        SearchCommand.prototype.configurationContainerLinks = configurationContainerLinks;

        navigationTreePluginArea.add(SearchCommand);
        assetsPanePluginArea.add(SearchCommand);
	}
});



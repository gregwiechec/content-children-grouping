define([
	"epi-cms/plugin-area/navigation-tree",
    "epi-cms/plugin-area/assets-pane",
    "./search-command"
], function (
    navigationTreePluginArea,
    assetsPanePluginArea,
    SearchCommand
) {
    return function () {
        navigationTreePluginArea.add(SearchCommand);
        assetsPanePluginArea.add(SearchCommand);
    };
});



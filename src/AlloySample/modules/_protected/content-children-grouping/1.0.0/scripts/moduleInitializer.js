define([
    "dojo/_base/declare",

    "epi/_Module",
    "epi-cms/ApplicationSettings",
    "./page-tree-initialization",
    "./search-command-initiaization",
	
	"./virtual-containers",

    "xstyle/css!./styles.css"
], function (
    declare,

    _Module,
    ApplicationSettings,
	
    pageTreeInitialization,
    searchCommandInitialization,
	virtualContainersInitialization
) {
    return declare([_Module], {
        initialize: function () {
            this.inherited(arguments);

            if (this._settings.customIconsEnabled) {
                pageTreeInitialization(this._settings.configurationContainerLinks);
            }
            if (this._settings.searchCommandEnabled) {
                ApplicationSettings.configurationContainerLinks = this._settings.configurationContainerLinks;
                searchCommandInitialization();
            }
			
			virtualContainersInitialization();
        }
    });
});
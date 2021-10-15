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

            ApplicationSettings.configurationContainerLinks = this._settings.configurationContainerLinks || [];
            ApplicationSettings.virtualContainerLinks = this._settings.virtualContainerLinks || [];
            ApplicationSettings.allConfigurationContainerLinks = (ApplicationSettings.configurationContainerLinks).concat(ApplicationSettings.virtualContainerLinks);

            if (this._settings.customIconsEnabled) {
                pageTreeInitialization();
            }
            if (this._settings.searchCommandEnabled) {
                searchCommandInitialization();
            }

            if (this._settings.virtualContainersEnabled) {
                virtualContainersInitialization();
            }
        }
    });
});
define([
    "dojo/_base/declare",

    "epi/_Module",
    "epi-cms/ApplicationSettings",
    "./page-tree-initialization",
    "./search-command-initiaization",

    "xstyle/css!./styles.css"
], function (
    declare,

    _Module,
    ApplicationSettings,
	
    pageTreeInitialization,
    searchCommandInitialization
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
        }
    });
});
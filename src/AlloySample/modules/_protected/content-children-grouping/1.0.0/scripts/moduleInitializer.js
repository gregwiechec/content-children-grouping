define([
    "dojo/_base/declare",

    "epi/_Module",
	"./page-tree-initialization"
], function (
    declare,

    _Module,
	
	pageTreeInitialization
) {
    return declare([_Module], {
        initialize: function () {
            this.inherited(arguments);

            pageTreeInitialization(this._settings.configurationContainerLinks);
        }
    });
});
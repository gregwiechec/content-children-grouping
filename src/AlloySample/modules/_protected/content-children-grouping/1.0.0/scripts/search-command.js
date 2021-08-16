define([
    "dojo/_base/declare",
    "epi/shell/command/_Command",
    "epi-cms/ApplicationSettings",
    "epi/shell/widget/dialog/Dialog",
    "epi-cms/_ContentContextMixin",
    "epi-cms/widget/SearchBox"
], function (
    declare,
    _Command,
	ApplicationSettings,
    Dialog,
    _ContentContextMixin,
    SearchBox
) {
    return declare([_Command, _ContentContextMixin], {
        label: "Search in container",
        iconClass: "epi-iconSearch",

        _onModelChange: function () {
            this.set("canExecute", true);

            this.set("isAvailable", false);

            var contentLink = this._getContentLink();
            if (!contentLink) {
                return;
            }

            var configurationContainerLinks = ApplicationSettings.configurationContainerLinks;
            this.set("isAvailable", configurationContainerLinks.indexOf(contentLink) !== -1);
        },

        _getContentLink: function () {
            var model;
            if (Array.isArray(this.model)) {
                if (this.model.length > 1) {
                    return;
                }
                model = this.model[0];
            } else {
                model = this.model;
            }
            if (!model) {
                return;
            }

            return model.contentLink;
        },

        contentContextChanged: function () {
            if (this._dialog) {
                this._dialog.hide();
            }
        },

        _execute: function () {
            var searchBox = new SearchBox({
                innerSearchBoxClass: "epi-search--full-width",
                triggerContextChange: true,
                triggerChangeOnEnter: true,
                encodeSearchText: true
            });
            searchBox.set("area", "cms/pages");
            searchBox.set("searchRoots", [this._getContentLink()]);

            this._dialog = new Dialog({
                dialogClass: "search-content-dialog",
                defaultActionsVisible: false,
                //confirmActionText: sharedResources.action.save,
                content: searchBox,
                //title: editsecurityResources.title
            });
            this._dialog.own(searchBox);

            this._dialog.show();
        }
    });
});
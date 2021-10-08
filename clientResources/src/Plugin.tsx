import React from "react";
// @ts-ignore
import { HashRouter as Router, Switch, Route } from "react-router-dom";
import App from "./App";
import { PluginInfo } from "./PluginInfo";
import { EditConfiguration } from "./edit-configuration";
import DataServiceContext, { dataService as defautDataService } from "./data-service";

export default function Plugin({ dataService }: AppProps) {
  if (!dataService) {
    dataService = defautDataService;
  }

  return (
    <DataServiceContext.Provider value={dataService}>
      <Router>
        <Switch>
          <Route exact path="/" render={(props: any) => <App {...props} />} />
          <Route path="/info" render={(props: any) => <PluginInfo {...props} />} />
          <Route path="/edit/:editContentLink" render={(props: any) => <EditConfiguration {...props} />} />
          <Route path="/add" render={(props: any) => <EditConfiguration {...props} />} />
        </Switch>
      </Router>
    </DataServiceContext.Provider>
  );
}

import React from "react";
// @ts-ignore
import { HashRouter as Router, Switch, Route } from "react-router-dom";
import App, { AppProps } from "./App";
import { PluginInfo } from "./PluginInfo";
import { EditConfiguration } from "./edit-configuration";

export default function Plugin({ dataService }: AppProps) {
  return (
    <Router>
      <Switch>
        <Route exact path="/" render={(props: any) => <App {...props} dataService={dataService} />} />
        <Route path="/info" render={(props: any) => <PluginInfo {...props} />} />
        <Route path="/edit/:editContentLink" render={(props: any) => <EditConfiguration {...props} />} />
        <Route path="/add" render={(props: any) => <EditConfiguration {...props} />} />
      </Switch>
    </Router>
  );
}

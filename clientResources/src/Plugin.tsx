import React from "react";
// @ts-ignore
import { HashRouter as Router, Switch, Route, Link } from "react-router-dom";
import App, { AppProps } from "./App";
import { PluginInfo } from "./PluginInfo";

export default function Plugin({ dataService }: AppProps) {
  return (
    <Router>
      <Switch>
        <Route exact path="/" render={(props: any) => <App {...props} dataService={dataService} />} />
        <Route path="/info" render={(props: any) => <PluginInfo {...props} />} />
      </Switch>
    </Router>
  );
}

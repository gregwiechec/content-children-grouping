import React, { useEffect, useState } from "react";
import { HashRouter as Router, Switch, Route, Link } from "react-router-dom";
import App, { AppProps } from "./App";

const Info = () => {
  return <div>TEST</div>;
};

export default function Plugin({ dataService }: AppProps) {
  return (
    <Router>
      <Switch>
        <Route path="/" render={props => <App {...props} dataService={dataService} />} />
        <Route path="/info" render={props => <Info {...props} />} />
      </Switch>
    </Router>
  );
}

import React, { useEffect, useState } from "react";
// @ts-ignore
import { HashRouter as Router, Switch, Route } from "react-router-dom";
import App from "./App";
import PluginInfo from "./components/plugin-info";
import EditConfigurationView from "./components/edit-configuration";
import DataServiceContext, { DataService, dataService as defaultDataService } from "./data-service";
import { Message } from "./Message";

interface PluginProps {
  dataService?: DataService | null;
}

let msgTimeoutHandler: any;

export default function Plugin({ dataService }: PluginProps) {
  if (!dataService) {
    dataService = defaultDataService;
  }

  const [message, setMessage] = useState("");

  useEffect(() => {
    setMessage(message);
    if (message) {
      if (msgTimeoutHandler) {
        clearTimeout(msgTimeoutHandler);
      }
      msgTimeoutHandler = setTimeout(() => setMessage(""), 5000);
    }

    return () => {
      if (msgTimeoutHandler) {
        clearTimeout(msgTimeoutHandler);
        msgTimeoutHandler = null;
      }
    };
  }, [message, msgTimeoutHandler]);

  const onMessageSet = (msg: string) => {
    setMessage(msg);
  };

  return (
    <>
      <Message message={message} />
      <DataServiceContext.Provider value={dataService}>
        <Router>
          <Switch>
            <Route exact path="/" render={(props: any) => <App {...props} onDeleteMessage={onMessageSet} />} />
            <Route path="/info" render={(props: any) => <PluginInfo {...props} />} />
            <Route
              path="/edit/:editContentLink"
              render={(props: any) => <EditConfigurationView {...props} onSaveSuccess={onMessageSet} />}
            />
            <Route
              path="/add"
              render={(props: any) => <EditConfigurationView {...props} onSaveSuccess={onMessageSet} />}
            />
          </Switch>
        </Router>
      </DataServiceContext.Provider>
    </>
  );
}

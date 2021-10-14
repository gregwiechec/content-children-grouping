import React, { useEffect, useState } from "react";
// @ts-ignore
import { HashRouter as Router, Switch, Route } from "react-router-dom";
import App from "./App";
import { PluginInfo } from "./PluginInfo";
import { EditConfigurationView } from "./edit-configuration/edit-configuration-view";
import DataServiceContext, { DataService, dataService as defaultDataService } from "./data-service";
import { Message } from "./Message";
import { ManageConfiguration } from "./manage-configuration";

interface PluginProps {
  dataService?: DataService | null;
}

export default function Plugin({ dataService }: PluginProps) {
  if (!dataService) {
    dataService = defaultDataService;
  }

  const [message, setMessage] = useState("");

  const [msgTimeoutHandler, setMsgTimeoutHandler] = useState<number | null>(null);


  useEffect(() => {
    setMessage(message);
    if (message) {
      if (msgTimeoutHandler) {
        clearTimeout(msgTimeoutHandler);
      }
      const handler = setTimeout(() => setMessage(""), 5000);
      // @ts-ignore
      setMsgTimeoutHandler(handler);
    }

    return () => {
      if (msgTimeoutHandler) {
        clearTimeout(msgTimeoutHandler);
        setMsgTimeoutHandler(null);
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
              path="/manage/:contentLink"
              render={(props: any) => <ManageConfiguration {...props} />}
            />
            <Route path="/add" render={(props: any) => <EditConfigurationView {...props} onSaveSuccess={onMessageSet} />} />
          </Switch>
        </Router>
      </DataServiceContext.Provider>
    </>
  );
}

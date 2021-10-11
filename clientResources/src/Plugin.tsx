import React, { useEffect, useState } from "react";
// @ts-ignore
import { HashRouter as Router, Switch, Route } from "react-router-dom";
import App from "./App";
import { PluginInfo } from "./PluginInfo";
import { EditConfiguration } from "./edit-configuration";
import DataServiceContext, { DataService, dataService as defaultDataService } from "./data-service";
import { Message } from "./Message";

interface PluginProps {
  dataService: DataService | null;
}

export default function Plugin({ dataService }: PluginProps) {
  if (!dataService) {
    dataService = defaultDataService;
  }

  const [message, setMessage] = useState("");

  let msgTimeoutHandler: number | null = null;
  useEffect(() => {
    setMessage(message);
    // @ts-ignore
    msgTimeoutHandler = setTimeout(() => setMessage(""), 5000);

    return () => {
      if (msgTimeoutHandler) {
        clearTimeout(msgTimeoutHandler);
        msgTimeoutHandler = null;
      }
    };
  }, [message]);

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
              render={(props: any) => <EditConfiguration {...props} onSaveSuccess={onMessageSet} />}
            />
            <Route path="/add" render={(props: any) => <EditConfiguration {...props} />} />
          </Switch>
        </Router>
      </DataServiceContext.Provider>
    </>
  );
}

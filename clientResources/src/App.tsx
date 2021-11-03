import React, { useEffect, useState } from "react";
// @ts-ignore
import { Link, useHistory } from "react-router-dom";
import { Attention, Button, Code, Grid, GridCell, GridContainer } from "optimizely-oui";
import "./App.scss";
import { ConfigurationsList } from "./components/configuration-list/configurations-list";
import { GroupConfiguration } from "./models/group-configuration";
import { useServerSettingsContext } from "./server-settings";
import { useDataServiceContext } from "./data-service";

interface AppProps {
  onDeleteMessage: (message: string) => void;
}

const App = ({ onDeleteMessage }: AppProps) => {
  const history = useHistory();
  const dataService = useDataServiceContext();
  const serverSettings = useServerSettingsContext();

  const [items, setItems] = useState<GroupConfiguration[]>([]);

  useEffect(() => {
    dataService?.load().then((result: any) => {
      setItems(result.items || []);
    });
  }, [dataService]);

  const onAddConfiguration = () => {
    history.push("/add");
  };

  const onDeleteConfiguration = (configuration: GroupConfiguration) => {
    dataService
      .delete(configuration)
      .then((result) => {
        onDeleteMessage("Configuration deleted");
        setItems(result);
      })
      .catch((error) => {
        onDeleteMessage(error.message);
      });

    const index = items.indexOf(configuration);
    if (index < 0) {
      return;
    }
  };

  return (
    <GridContainer>
      <Grid>
        <GridCell large={12} medium={8} small={4}>
          <ConfigurationsList
            items={items}
            onEdit={(c) => history.push("/edit/" + c.contentLink)}
            onManage={(c) => history.push("/manage/" + c.contentLink)}
            onDelete={onDeleteConfiguration}
          />
        </GridCell>
        <GridCell large={12} medium={8} small={4}>
          {serverSettings.options.databaseConfigurationsEnabled && (
            <Button
              className="add-configuration-button"
              style="highlight"
              size="narrow"
              leftIcon="add"
              onClick={onAddConfiguration}
            >
              Add configuration
            </Button>
          )}
        </GridCell>
        {!serverSettings.options.databaseConfigurationsEnabled && (
          <GridCell large={12} medium={8} small={4}>
            <Attention type="warning" alignment="left">
              Database registration is not enabled and you can't add configurations. Only configuration registered from code are available.
              <br />
              You need to turn it on through code using options:
              <br />
              <Code className="sample-code">
                context.Services.AddTransient(serviceLocator ={">"} new ContentChildrenGroupingOptions
                <br />
                {"{"}
                <br />
                &nbsp;&nbsp;&nbsp;&nbsp;DatabaseConfigurationsEnabled = true
                <br />
                {"}"});
                <br />
              </Code>
            </Attention>
          </GridCell>
        )}

        <GridCell large={12} medium={8} small={4}>
          <Link to="/info">Show plugin information</Link>
        </GridCell>
      </Grid>
    </GridContainer>
  );
};

export default App;

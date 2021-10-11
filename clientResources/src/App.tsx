import React, { useEffect, useState } from "react";
// @ts-ignore
import { Link, useHistory } from "react-router-dom";
import { Attention, Button, Code } from "optimizely-oui";
import "./App.scss";
import { ConfigurationsList } from "./configurations-list";
import { GroupConfiguration } from "./models/Groupconfiguration";
import { ManageConfigurationDialog } from "./manage-configuration-dialog";
import { useServerSettingsContext } from "./server-settings";
import { useDataServiceContext } from "./data-service";

let successTimeoutHandle: number;

interface AppProps {
  onDeleteMessage: (message: string) => void;
}

const App = ({ onDeleteMessage }: AppProps) => {
  const history = useHistory();
  const dataService = useDataServiceContext();
  const serverSettings = useServerSettingsContext();

  //TODO: remove
  const [currentConfiguration, setCurrentConfiguration] = useState<GroupConfiguration | null>(null);

  const [currentManageConfiguration, setCurrentManageConfiguration] = useState<GroupConfiguration | null>(null);

  //TODO: remove
  const [isNewConfiguration, setIsNewConfiguration] = useState(false);

  //TODO: remove
  const [dialogValidationError, setDialogValidationError] = useState("");
  const [saveMessage, setSaveMessage] = useState("");
  const [saveMessageType, setSaveMessageType] = useState<"bad-news" | "brand" | "good-news" | "warning">("good-news");

  const [items, setItems] = useState<GroupConfiguration[]>([]);

  useEffect(() => {
    dataService?.load().then((result: any) => {
      setItems(result.items || []);
    });

    return () => {
      if (successTimeoutHandle) {
        clearTimeout(successTimeoutHandle);
      }
    };
  }, [dataService]);

  const onAddConfiguration = () => {
    history.push("/add");
  };

  //TODO: move validation
  const onDialogSave = (configuration: GroupConfiguration): void => {
    if (!currentConfiguration) {
      return;
    }

    let updatedList: GroupConfiguration[];
    if (isNewConfiguration) {
      // add new item to array
      let config = items.filter((x) => x.contentLink === configuration.contentLink)[0];
      if (config != null) {
        setDialogValidationError("Duplicated configuration container");
        return;
      }
      updatedList = [...items, configuration];
    } else {
      let existingConfig = items.filter((x) => x.contentLink === configuration.contentLink)[0];
      if (existingConfig) {
        if (existingConfig !== currentConfiguration) {
          setDialogValidationError("Duplicated configuration container");
          return;
        }
      }
      const index = items.indexOf(currentConfiguration);
      updatedList = [...items];
      const c = Object.assign({}, updatedList[index]);
      c.contentLink = configuration.contentLink;
      c.containerTypeName = configuration.containerTypeName;
      c.routingEnabled = configuration.routingEnabled;
      c.groupLevelConfigurations = configuration.groupLevelConfigurations;
      updatedList[index] = c;
    }

    setItems(updatedList);
    setCurrentConfiguration(null);
    setDialogValidationError("");
  };

  const onManageConfiguration = (configuration: GroupConfiguration) => {
    setCurrentManageConfiguration(configuration);
  };

  const onDeleteConfiguration = (configuration: GroupConfiguration) => {
    dataService
      .delete(configuration)
      .then(result => {
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

  const onSaveClick = () => {
    setSaveMessage("Saving");
    setSaveMessageType("brand");
    dataService
      ?.save(items)
      .then((response) => {
        setSaveMessageType("good-news");
        setSaveMessage("Configuration saved");
      })
      .catch((error) => {
        setSaveMessageType("bad-news");
        setSaveMessage("Saving configuration failed");
      });
  };

  return (
    <div className="App">
      {saveMessage && (
        <Attention type={saveMessageType} alignment="center">
          {saveMessage}
        </Attention>
      )}

      <Link to="/info">Show plugin configuration</Link>
      <br />
      <br />

      <ConfigurationsList
        items={items}
        onEdit={(c) => history.push("/edit/" + c.contentLink)}
        onManage={onManageConfiguration}
        onDelete={onDeleteConfiguration}
      />
      {serverSettings.options.databaseConfigurationsEnabled && (
        <Button
          className="add-configuration-button"
          style="outline"
          size="narrow"
          leftIcon="add"
          onClick={onAddConfiguration}
        >
          Add
        </Button>
      )}

      {!serverSettings.options.databaseConfigurationsEnabled && (
        <>
          <br />
          <br />
          <Attention type="warning" alignment="left">
            Database registration is not enabled. You need to turn it on through code using options:
            <br />
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
        </>
      )}

      {!!currentManageConfiguration && (
        <ManageConfigurationDialog
          dataService={dataService}
          structureUpdateEnabled={serverSettings.options.structureUpdateEnabled}
          configuration={currentManageConfiguration}
          onCancel={() => setCurrentManageConfiguration(null)}
        />
      )}
    </div>
  );
};

export default App;

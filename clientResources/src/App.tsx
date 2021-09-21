import React, { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { Attention, Button, Code } from "optimizely-oui";
import "./App.scss";
import { ConfigurationsList } from "./configurations-list";
import { GroupConfiguration } from "./models/Groupconfiguration";
import { EditConfigurationDialog } from "./edit-configuration-dialog";
import { DataService, dataService as defaultDataService } from "./data-service";
import { ManageConfigurationDialog } from "./manage-configuration-dialog";
import {useServerSettingsContext} from "./server-settings";

export interface AppProps {
  dataService?: DataService;
}

let successTimeoutHandle: number;

const App = ({
  dataService,
}: AppProps) => {
  const serverSettings = useServerSettingsContext();

  const [currentConfiguration, setCurrentConfiguration] = useState<GroupConfiguration | null>(null);
  const [currentManageConfiguration, setCurrentManageConfiguration] = useState<GroupConfiguration | null>(null);
  const [isNewConfiguration, setIsNewConfiguration] = useState(false);
  const [dialogValidationError, setDialogValidationError] = useState("");
  const [saveMessage, setSaveMessage] = useState("");
  const [saveMessageType, setSaveMessageType] = useState<"bad-news" | "brand" | "good-news" | "warning">("good-news");

  const [items, setItems] = useState<GroupConfiguration[]>([]);

  if (!dataService) {
    dataService = defaultDataService;
  }

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
    setDialogValidationError("");
    setIsNewConfiguration(true);
    setCurrentConfiguration({
      contentLink: "",
      fromCode: false,
      containerTypeName: "",
      routingEnabled: true,
      groupLevelConfigurations: [serverSettings.availableNameGenerators[0]] || []
    });
  };

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

  const onEditConfiguration = (configuration: GroupConfiguration) => {
    setIsNewConfiguration(false);
    setDialogValidationError("");
    setCurrentConfiguration(configuration);
  };

  const onManageConfiguration = (configuration: GroupConfiguration) => {
    setCurrentManageConfiguration(configuration);
  };

  const onDeleteConfiguration = (configuration: GroupConfiguration) => {
    const index = items.indexOf(configuration);
    if (index < 0) {
      return;
    }
    const itemsCopy = [...items];
    itemsCopy.splice(index, 1);
    setItems(itemsCopy);
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

      <Link to="/info">Check plugin configuration</Link>

      <ConfigurationsList
        items={items}
        onEdit={onEditConfiguration}
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

      {serverSettings.options.databaseConfigurationsEnabled && (
        <Button className="save-button" style="highlight" size="narrow" leftIcon="save" onClick={onSaveClick}>
          Save configurations
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

      {!!currentConfiguration && (
        <EditConfigurationDialog
          configuration={currentConfiguration}
          validationMessage={dialogValidationError}
          onSave={onDialogSave}
          onCancel={() => setCurrentConfiguration(null)}
          availableNameGenerators={serverSettings.availableNameGenerators}
        />
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

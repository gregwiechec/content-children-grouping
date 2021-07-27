import React, { useEffect, useState } from "react";
import { Attention, Button } from "optimizely-oui";
import "./App.scss";
import { ConfigurationsList } from "./configurations-list";
import { GroupConfiguration } from "./models/Groupconfiguration";
import { EditConfigurationDialog } from "./edit-configuration-dialog";
import { DataService, dataService as defaultDataService } from "./data-service";

interface AppProps {
  dataService?: DataService;
}

let successTimeoutHandle: number;

const App = ({ dataService }: AppProps) => {
  const [currentConfiguration, setCurrentConfiguration] = useState<GroupConfiguration | null>(null);
  const [isNewConfiguration, setIsNewConfiguration] = useState(false);
  const [dialogValidationError, setDialogValidationError] = useState("");
  const [showSaveMessage, setShowSaveMessage] = useState(false);

  const [items, setItems] = useState<GroupConfiguration[]>([]);
  const [availableNameGenerators, setAvailableGenerators] = useState<string[]>([]);

  if (!dataService) {
    dataService = defaultDataService;
  }

  useEffect(() => {
    dataService?.load().then((result: any) => {
      setItems(result.items || []);
      setAvailableGenerators(result.availableNameGenerators || []);
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
      containerTypeName: "",
      routingEnabled: true,
      groupLevelConfigurations: [availableNameGenerators[0]] || []
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
    setShowSaveMessage(true);
    // @ts-ignore
    successTimeoutHandle = setTimeout(() => {
      setShowSaveMessage(false);
    }, 3000);
  };

  return (
    <div className="App">
      {showSaveMessage && <Attention alignment="center">Configuration saved</Attention>}

      <ConfigurationsList items={items} onEdit={onEditConfiguration} onDelete={onDeleteConfiguration} />
      <br />
      <Button
        className="add-configuration-button"
        style="outline"
        size="narrow"
        leftIcon="add"
        onClick={onAddConfiguration}
      >
        Add configuration
      </Button>

      <Button style="highlight" size="narrow" leftIcon="save" onClick={onSaveClick}>
        Save
      </Button>

      {!!currentConfiguration && (
        <EditConfigurationDialog
          configuration={currentConfiguration}
          validationMessage={dialogValidationError}
          onSave={onDialogSave}
          onCancel={() => setCurrentConfiguration(null)}
          availableNameGenerators={availableNameGenerators}
        />
      )}
    </div>
  );
};

export default App;

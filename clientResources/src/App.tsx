import React, { useState } from "react";
import { Attention, Button } from "optimizely-oui";
import "./App.scss";
import { ConfigurationsList } from "./configurations-list";
import { GroupConfiguration } from "./models/Groupconfiguration";
import { EditConfigurationDialog } from "./edit-configuration-dialog";

interface AppProps {
  items: GroupConfiguration[];
  availableNameGenerators: string[];
}

const App = ({ items, availableNameGenerators }: AppProps) => {
  const [currentItems, setCurrentItems] = useState(items || []);
  const [currentConfiguration, setCurrentConfiguration] = useState<GroupConfiguration | null>(null);
  const [isNewConfiguration, setIsNewConfiguration] = useState(false);
  const [dialogValidationError, setDialogValidationError] = useState("");
  const [showSaveMessage, setShowSaveMessage] = useState(false);

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
      let config = currentItems.filter(x=> x.contentLink === configuration.contentLink)[0];
      if (config != null) {
        setDialogValidationError("Duplicated configuration container");
        return ;
      }
      updatedList = [...currentItems, configuration];
    } else {
      let existingConfig = currentItems.filter(x=> x.contentLink === configuration.contentLink)[0];
      if (existingConfig){
        if (existingConfig !== currentConfiguration) {
          setDialogValidationError("Duplicated configuration container");
          return;
        }
      }
      const index = currentItems.indexOf(currentConfiguration);
      updatedList = [...currentItems];
      const c = Object.assign({},updatedList[index]);
      c.contentLink = configuration.contentLink;
      c.containerTypeName = configuration.containerTypeName;
      c.routingEnabled = configuration.routingEnabled;
      c.groupLevelConfigurations = configuration.groupLevelConfigurations;
      updatedList[index] = c;
    }

    setCurrentItems(updatedList);
    setCurrentConfiguration(null);
    setDialogValidationError("");
  };

  const onEditConfiguration = (configuration: GroupConfiguration) => {
    setIsNewConfiguration(false);
    setDialogValidationError("");
    setCurrentConfiguration(configuration);
  };

  const onDeleteConfiguration = (configuration: GroupConfiguration) => {
    const index = currentItems.indexOf(configuration);
    if (index < 0) {
      return;
    }
    const itemsCopy = [...currentItems];
    itemsCopy.splice(index, 1);
    setCurrentItems(itemsCopy);
  };

  const onSaveClick = () => {
    alert(1);
  };

  return (
    <div className="App">
      <Attention alignment="center" isDismissible>
        Configuration saved
      </Attention>

      <ConfigurationsList items={currentItems} onEdit={onEditConfiguration} onDelete={onDeleteConfiguration} />

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

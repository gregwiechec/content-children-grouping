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
  const [showSaveMessage, setShowSaveMessage] = useState(false);

  const onAddConfiguration = () => {
    setIsNewConfiguration(true);
      setCurrentConfiguration({
          contentLink: "",
          containerTypeName: "",
          routingEnabled: true,
          groupLevelConfigurations: [availableNameGenerators[0]] || []
      });
  };

  const onDialogSave = (configuration: GroupConfiguration) => {
    const updatedList = [...currentItems, configuration];
    setCurrentItems(updatedList);
    setCurrentConfiguration(null);
  };

  const onSaveClick = () => {
    alert(1);
  };

  return (
    <div className="App">
      <Attention alignment="center" isDismissible>
        Configuration saved
      </Attention>

      <ConfigurationsList items={currentItems} />

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
          onSave={onDialogSave}
          onCancel={() => setCurrentConfiguration(null)}
          availableNameGenerators={availableNameGenerators}
        />
      )}
    </div>
  );
};

export default App;

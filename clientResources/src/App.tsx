import React from "react";
import { Attention, Button, GridCell } from "optimizely-oui";
import "./App.scss";
import { ConfigurationsList } from "./configurations-list";
import { GroupConfiguration } from "./models/Groupconfiguration";

interface AppProps {
  items: GroupConfiguration[];
  availableNameGenerators: string[];
}

const App = ({ items, availableNameGenerators }: AppProps) => {
  const onListChange = () => {};

  const onSaveClick = () => {
    alert(1);
  };

  return (
    <div className="App">
      <Attention alignment="center" isDismissible>
        Configuration saved
      </Attention>

      <ConfigurationsList items={items} availableNameGenerators={availableNameGenerators} onListChange={onListChange} />

      <Button className="add-configuration-button" style="outline" size="narrow" leftIcon="add" onClick={onSaveClick}>
        Add configuration
      </Button>

      <Button style="highlight" size="narrow" leftIcon="save" onClick={onSaveClick}>
        Save
      </Button>
    </div>
  );
};

export default App;

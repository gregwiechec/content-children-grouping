import React from "react";
import { Attention } from "optimizely-oui";
import "./App.scss";
import { ConfigurationsList } from "./configurations-list";

function App() {
  const onListChange = () => {};

  return (
    <div className="App">
      <Attention alignment="center" isDismissible>
        Configuration saved
      </Attention>

      <ConfigurationsList items={[]} availableNameGenerators={[]} onListChange={onListChange} />
    </div>
  );
}

export default App;

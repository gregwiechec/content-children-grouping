import React from "react";
import ReactDOM from "react-dom";
import "./index.css";
import App from "./App";
import reportWebVitals from "./reportWebVitals";
import axios from "axios";

const rootElement = document.getElementById("root");
const configuration = JSON.parse(rootElement?.dataset?.configuration || "{}");
axios.defaults.baseURL = configuration.baseUrl;

ReactDOM.render(
  <React.StrictMode>
    <App
      structureUpdateEnabled={configuration.structureUpdateEnabled}
      availableNameGenerators={configuration.availableNameGenerators}
      databaseConfigurationsEnabled={configuration.databaseConfigurationsEnabled}
    />
  </React.StrictMode>,
  document.getElementById("root")
);

// If you want to start measuring performance in your app, pass a function
// to log results (for example: reportWebVitals(console.log))
// or send to an analytics endpoint. Learn more: https://bit.ly/CRA-vitals
reportWebVitals();

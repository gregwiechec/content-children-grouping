import React from "react";
import ReactDOM from "react-dom";
import App from "./App";
import reportWebVitals from "./reportWebVitals";
import axios from "axios";
import ServerSettingsContext, {ServerSettings} from "./server-settings";
import "./index.css";

const rootElement = document.getElementById("root");
const configuration = JSON.parse(rootElement?.dataset?.configuration || "{}");
axios.defaults.baseURL = configuration.baseUrl;

const settings: ServerSettings = {
    structureUpdateEnabled: configuration.structureUpdateEnabled,
    availableNameGenerators: configuration.availableNameGenerators,
    databaseConfigurationsEnabled: configuration.databaseConfigurationsEnabled,
    contentUrl: configuration.contentUrl,
    defaultContainerType: configuration.defaultContainerType
};

ReactDOM.render(
    <React.StrictMode>
        <ServerSettingsContext.Provider value={settings}>
            <App />
        </ServerSettingsContext.Provider>
    </React.StrictMode>,
    document.getElementById("root")
);

// If you want to start measuring performance in your app, pass a function
// to log results (for example: reportWebVitals(console.log))
// or send to an analytics endpoint. Learn more: https://bit.ly/CRA-vitals
reportWebVitals();

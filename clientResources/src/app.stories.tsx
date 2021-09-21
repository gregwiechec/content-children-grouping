import React from "react";
import { ComponentStory, ComponentMeta } from "@storybook/react";
import { fakeService } from "./fake-data/fake-service";
import App from "./App";
import "./App.scss";
import ServerSettingsContext from "./server-settings";
import {DataService} from "./data-service";

interface ComponentProps {
  structureUpdateEnabled: boolean;
  availableNameGenerators: string[];
  databaseConfigurationsEnabled: boolean;
  contentUrl: string;
  defaultContainerType: string;
  dataService: DataService;
}

const Component = (settings: ComponentProps) => {
  return (
    <ServerSettingsContext.Provider value={settings}>
      <App dataService={settings.dataService}/>
    </ServerSettingsContext.Provider>
  );
};

export default {
  title: "App",
  component: Component
} as ComponentMeta<typeof Component>;

const Template: ComponentStory<typeof Component> = (args) => <Component {...args} />;

export const AppStory = Template.bind({});
AppStory.args = {
  dataService: fakeService,
  availableNameGenerators: ["Name", "Created Date", "Very long name generator"],
  databaseConfigurationsEnabled: true,
  structureUpdateEnabled: true
};

const emptyService = {
  load: () => {
    return new Promise((resolve) => {
      setTimeout(() => {
        resolve({ items: [] });
      }, 500);
    });
  },
  save: () => new Promise((resolve) => resolve(true)),
  clearContainers: () => new Promise((resolve) => resolve("test"))
};

export const EmptyApp = Template.bind({});
EmptyApp.args = {
  dataService: emptyService,
  availableNameGenerators: ["Name", "Created Date", "Very long name generator"],
  databaseConfigurationsEnabled: true,
  structureUpdateEnabled: true
};

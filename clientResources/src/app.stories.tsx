import React from "react";
import { ComponentStory, ComponentMeta } from "@storybook/react";
import { fakeService } from "./fake-data/fake-service";
import App from "./App";
import "./App.scss";

export default {
  title: "App",
  component: App
} as ComponentMeta<typeof App>;

const Template: ComponentStory<typeof App> = (args) => <App {...args} />;

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
  save: () => new Promise(resolve => resolve(true)),
  clearContainers: () => new Promise(resolve => resolve("test"))
};

export const EmptyApp = Template.bind({ });
EmptyApp.args = {
  dataService: emptyService,
  availableNameGenerators: ["Name", "Created Date", "Very long name generator"],
  databaseConfigurationsEnabled: true,
  structureUpdateEnabled: true
}

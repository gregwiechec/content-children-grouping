import React from "react";
import { ComponentStory, ComponentMeta } from "@storybook/react";
import { fakeService } from "./fake-data/fake-service";
import Plugin from "./Plugin";
import "./App.scss";
import ServerSettingsContext, { ServerSettings } from "./server-settings";
import { DataService } from "./data-service";

interface ComponentProps extends ServerSettings {
  dataService: DataService;
}

const Component = (settings: ComponentProps) => {
  return (
    <ServerSettingsContext.Provider value={settings}>
      <Plugin dataService={settings.dataService} />
    </ServerSettingsContext.Provider>
  );
};

export default {
  title: "App",
  component: Component
} as ComponentMeta<typeof Component>;

const Template: ComponentStory<typeof Component> = (args) => <Component {...args} />;

const getDefaultProps = (dataService: DataService) => {
  return {
    dataService: dataService,
    availableNameGenerators: ["Name", "Created Date", "Very long name generator"],
    defaultContainerType: "",
    contentUrl: "",
    options: {
      customIconsEnabled: false,
      databaseConfigurationsEnabled: false,
      routerEnabled: false,
      searchCommandEnabled: false,
      structureUpdateEnabled: false
    }
  };
};

export const AppStory = Template.bind({});
AppStory.args = getDefaultProps(fakeService);

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
EmptyApp.args = getDefaultProps(emptyService);

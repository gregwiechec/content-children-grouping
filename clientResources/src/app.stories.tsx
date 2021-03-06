import React from "react";
import { ComponentStory, ComponentMeta } from "@storybook/react";
import { fakeService } from "./fake-data/fake-service";
import Plugin from "./Plugin";
import "./App.scss";
import ServerSettingsContext, { ServerSettings } from "./server-settings";
import { DataService } from "./data-service";
import { GroupConfiguration } from "./models/group-configuration";

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

const getDefaultProps = (dataService: DataService, databaseConfigurationsEnabled = true, virtualContainersEnabled = true) => {
  return {
    dataService: dataService,
    availableNameGenerators: ["Name", "Create Date", "Very long name generator"],
    contentUrl: "http://google.com/{contentLink}",
    options: {
      customIconsEnabled: false,
      databaseConfigurationsEnabled: databaseConfigurationsEnabled,
      searchCommandEnabled: false,
      virtualContainersEnabled: virtualContainersEnabled
    }
  };
};

export const AppStory = Template.bind({});
AppStory.args = getDefaultProps(fakeService);

const emptyService: DataService = {
  load: () => {
    return new Promise((resolve) => {
      setTimeout(() => {
        resolve({ items: [] });
      }, 500);
    });
  },
  save: () => new Promise((resolve) => resolve(true)),
  get: (contentLink: string) =>
    new Promise((resolve) =>
      resolve({
        contentLink: contentLink,
        fromCode: false,
        groupLevelConfigurations: []
      })
    ),
  delete(configuration: GroupConfiguration): Promise<GroupConfiguration[]> {
    return new Promise((resolve) => resolve([]));
  }
};

export const EmptyApp = Template.bind({});
EmptyApp.args = getDefaultProps(emptyService);

export const NoDataBaseConfiguration = Template.bind({});
NoDataBaseConfiguration.args = getDefaultProps(fakeService, false);

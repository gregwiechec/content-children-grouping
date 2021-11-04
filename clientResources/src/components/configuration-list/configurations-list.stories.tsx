import React from "react";
import { ComponentStory, ComponentMeta } from "@storybook/react";
import { ConfigurationsList, ConfigurationsListProps } from "./configurations-list";
import "./../../App.scss";
import ServerSettingsContext from "../../server-settings";
import { getServerSettings } from "../../fake-data/fake-server-settings";
import { action } from "@storybook/addon-actions";

interface ConfigurationListComponentStorybookProps extends ConfigurationsListProps {
  router: boolean;
  virtualContainersEnabled: boolean;
  physicalContainersEnabled: boolean;
}

const ConfigurationListStorybook = (props: ConfigurationListComponentStorybookProps) => {
  let overridden: any = {};
  if (props.router === false) {
    overridden.routerEnabled = false;
  }
  if (props.virtualContainersEnabled === false) {
    overridden.virtualContainersEnabled = false;
  }
  if (props.physicalContainersEnabled === false) {
    overridden.physicalContainersEnabled = false;
  }
  const serverSettings = getServerSettings(overridden);

  return (
    <div style={{ width: 1000 }}>
      <ServerSettingsContext.Provider value={serverSettings}>
        <ConfigurationsList
          items={getItems()}
          onManage={action("manage")}
          onDelete={action("delete")}
          onEdit={action("edit")}
        />
      </ServerSettingsContext.Provider>
    </div>
  );
};

export default {
  title: "Configuration list",
  component: ConfigurationListStorybook
} as ComponentMeta<typeof ConfigurationListStorybook>;

const Template: ComponentStory<typeof ConfigurationListStorybook> = (args) => <ConfigurationListStorybook {...args} />;

const getItems = () => {
  return [
    {
      contentLink: "123",
      fromCode: false,
      routingEnabled: false,
      containerTypeName: "",
      groupLevelConfigurations: [{ name: "Name" }, { name: "CreatedDate" }],
      isVirtualContainer: true
    },
    {
      contentLink: "124",
      fromCode: false,
      routingEnabled: true,
      containerTypeName: "AlloySample.Models.Pages.ContainerPage, AlloySample",
      groupLevelConfigurations: [{ name: "Name" }],
      isVirtualContainer: false
    }
  ];
};

export const DefaultList = Template.bind({});

export const WithNoRouter = Template.bind({});
WithNoRouter.args = {
  router: false
};

export const WithNoVirtualContainers = Template.bind({});
WithNoVirtualContainers.args = {
  virtualContainersEnabled: false
};

export const WithNoPhysicalContainers = Template.bind({});
WithNoPhysicalContainers.args = {
  physicalContainersEnabled: false
};

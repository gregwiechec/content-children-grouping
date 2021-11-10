import React from "react";
import { ComponentStory, ComponentMeta } from "@storybook/react";
import { ConfigurationsList, ConfigurationsListProps } from "./configurations-list";
import "./../../App.scss";
import ServerSettingsContext from "../../server-settings";
import { getServerSettings } from "../../fake-data/fake-server-settings";
import { action } from "@storybook/addon-actions";

interface ConfigurationListComponentStorybookProps extends ConfigurationsListProps {
  virtualContainersEnabled: boolean;
}

const ConfigurationListStorybook = (props: ConfigurationListComponentStorybookProps) => {
  let overridden: any = {};
  if (props.virtualContainersEnabled === false) {
    overridden.virtualContainersEnabled = false;
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
      groupLevelConfigurations: [{ name: "Name" }, { name: "CreatedDate" }]
    },
    {
      contentLink: "124",
      fromCode: false,
      groupLevelConfigurations: [{ name: "Name" }]
    }
  ];
};

export const DefaultList = Template.bind({});

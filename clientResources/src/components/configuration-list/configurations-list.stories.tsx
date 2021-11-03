import React from "react";
import { ComponentStory, ComponentMeta } from "@storybook/react";
import { ConfigurationsList } from "./configurations-list";
import "./../../App.scss";
import ServerSettingsContext from "../../server-settings";
import { getServerSettings } from "../../fake-data/fake-server-settings";

export default {
  title: "Configuration list",
  component: ConfigurationsList
} as ComponentMeta<typeof ConfigurationsList>;

const Template: ComponentStory<typeof ConfigurationsList> = (args: any) => {
  var overridden: any = {};
  if (args.router === false) {
    overridden.routerEnabled = false;
  }
  if (args.virtualContainersEnabled === false) {
    overridden.virtualContainersEnabled = false;
  }
  const serverSettings = getServerSettings(overridden);

  return (
    <div style={{ width: 1000 }}>
      <ServerSettingsContext.Provider value={serverSettings}>
        <ConfigurationsList {...args} />
      </ServerSettingsContext.Provider>
    </div>
  );
};

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
  ]
};

export const DefaultList = Template.bind({});
DefaultList.args = {
  items: getItems()
};

export const WithNoRouter = Template.bind({});
WithNoRouter.args = {
  items: getItems(),
  router: false
};

export const WithNoVirtualcontainers = Template.bind({});
WithNoVirtualcontainers.args = {
  items: getItems(),
  virtualContainersEnabled: false
};

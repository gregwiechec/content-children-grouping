import React from "react";
import { ComponentStory, ComponentMeta } from "@storybook/react";
import { ConfigurationsList } from "./configurations-list";
import "./App.scss";

export default {
  title: "List",
  component: ConfigurationsList
} as ComponentMeta<typeof ConfigurationsList>;

const Template: ComponentStory<typeof ConfigurationsList> = (args) => <ConfigurationsList {...args} />;

export const DefaultList = Template.bind({});
DefaultList.args = {
  items: [
    {
      contentLink: "123",
      routingEnabled: false,
      containerTypeName: "",
      groupLevelConfigurations: ["Name", "CreatedDate"]
    },
    {
      contentLink: "124",
      routingEnabled: true,
      containerTypeName: "",
      groupLevelConfigurations: ["Name"]
    }
  ]
};

export const AdminMode = (args: any) => (
  <div style={{ width: 1000 }}>
    <ConfigurationsList {...args} />
  </div>
);

AdminMode.args = {
  items: [
    {
      contentLink: "123",
      routingEnabled: false,
      containerTypeName: "AlloySample.Models.Pages.ContainerPage, AlloySample, Version=1.0.0.0, Culture=neutral",
      groupLevelConfigurations: ["Name", "CreatedDate"]
    },
    {
      contentLink: "124",
      routingEnabled: true,
      containerTypeName: "",
      groupLevelConfigurations: ["Name", "Test1", "Test2", "Test3", "Test4"]
    }
  ]
};

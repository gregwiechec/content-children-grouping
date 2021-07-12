import React from "react";
import { ComponentStory, ComponentMeta } from "@storybook/react";

import { ConfigurationsList } from "./configurations-list";
import "./App.scss";

export default {
  title: "Configuraion/List",
  component: ConfigurationsList
} as ComponentMeta<typeof ConfigurationsList>;

const Template: ComponentStory<typeof ConfigurationsList> = (args) => <ConfigurationsList {...args} />;

export const LoggedIn = Template.bind({});
LoggedIn.args = {
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
  ],
  availableNameGenerators: ["name", "created date", "very long name generator"]
};

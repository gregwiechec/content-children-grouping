import React from "react";
import { ComponentStory, ComponentMeta } from "@storybook/react";

import { EditConfiguration } from "./edit-configuration";
import "./App.scss";
import { action } from "@storybook/addon-actions";

export default {
  title: "Edit Dialog",
  component: EditConfiguration
} as ComponentMeta<typeof EditConfiguration>;

const Template: ComponentStory<typeof EditConfiguration> = (args) => (
  <EditConfiguration
    {...args}
    onSaveSuccess={action("onSave")}
  />
);

export const DefaultEditConfigurationDialog = Template.bind({});

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
  <EditConfiguration {...args} availableNameGenerators={["name", "created date", "long generator name test test test test"]} onSave={action("onSave")} onCancel={action("onCancel")} />
);

export const DefaultEditConfigurationDialog = Template.bind({});

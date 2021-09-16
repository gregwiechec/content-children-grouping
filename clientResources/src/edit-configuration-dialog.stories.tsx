import React from "react";
import { ComponentStory, ComponentMeta } from "@storybook/react";

import { EditConfigurationDialog } from "./edit-configuration-dialog";
import "./App.scss";
import { action } from "@storybook/addon-actions";

export default {
  title: "Edit Dialog",
  component: EditConfigurationDialog
} as ComponentMeta<typeof EditConfigurationDialog>;

const Template: ComponentStory<typeof EditConfigurationDialog> = (args) => (
  <EditConfigurationDialog {...args} availableNameGenerators={["name", "created date", "long generator name test test test test"]} onSave={action("onSave")} onCancel={action("onCancel")} />
);

export const DefaultEditConfigurationDialog = Template.bind({});

import React from "react";
import { ComponentStory, ComponentMeta } from "@storybook/react";

import { EditConfigurationView } from "./edit-configuration-view";
import "../App.scss";
import { action } from "@storybook/addon-actions";

export default {
  title: "Edit Dialog",
  component: EditConfigurationView
} as ComponentMeta<typeof EditConfigurationView>;

const Template: ComponentStory<typeof EditConfigurationView> = (args) => (
  <EditConfigurationView
    {...args}
    onSaveSuccess={action("onSave")}
  />
);

export const DefaultEditConfigurationDialog = Template.bind({});

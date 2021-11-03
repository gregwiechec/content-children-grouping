import React from "react";
import { ComponentStory, ComponentMeta } from "@storybook/react";
// @ts-ignore
import { MemoryRouter, Route} from "react-router-dom";
import { EditConfiguration } from "./edit-configuration";
import "../../App.scss";
import { action } from "@storybook/addon-actions";

export default {
  title: "Edit Dialog",
  component: EditConfiguration,
  decorators: [(Story) => (
      <MemoryRouter initialEntries={["/path/12345"]}>
        <Route path="/path/:editContentLink">
          <Story />
        </Route>
      </MemoryRouter>)],
} as ComponentMeta<typeof EditConfiguration>;

const Template: ComponentStory<typeof EditConfiguration> = (args) => (
  <EditConfiguration
    {...args}
    onSaveSuccess={action("onSave")}
  />
);

export const DefaultEditConfigurationDialog = Template.bind({});

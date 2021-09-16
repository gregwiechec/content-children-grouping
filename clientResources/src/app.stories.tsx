import React from "react";
import { ComponentStory, ComponentMeta } from "@storybook/react";
import { fakeService } from "./fake-data/fake-service";
import App from "./App";
import "./App.scss";

export default {
  title: "App",
  component: App
} as ComponentMeta<typeof App>;

const Template: ComponentStory<typeof App> = (args) => <App {...args} />;

export const AppStory = Template.bind({});
AppStory.args = {
  dataService: fakeService
};

const emptyService = {
  load: () => {
    return new Promise((resolve) => {
      setTimeout(() => {
        resolve({ items: [], availableNameGenerators: ["Name", "Created Date", "Very long name generator"] });
      }, 500);
    });
  }
};

export const EmptyApp = Template.bind({ dataService: emptyService });

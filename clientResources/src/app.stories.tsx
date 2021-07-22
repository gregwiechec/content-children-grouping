import React from "react";
import { ComponentStory, ComponentMeta } from "@storybook/react";

import App from "./App";
import "./App.scss";

export default {
  title: "App",
  component: App
} as ComponentMeta<typeof App>;

const service = {
  load: () => {
    return new Promise((resolve) => {
      setTimeout(() => {
        var result = {
          items: [
            {
              contentLink: "123",
              routingEnabled: false,
              containerTypeName: "",
              groupLevelConfigurations: ["Name", "Created Date"]
            },
            {
              contentLink: "124",
              routingEnabled: true,
              containerTypeName: "",
              groupLevelConfigurations: ["Name"]
            }
          ],
          availableNameGenerators: ["Name", "Created Date", "Very long name generator"]
        };
        resolve(result);
      }, 500);
    });
  }
};

const Template: ComponentStory<typeof App> = (args) => <App dataService={service} />;

export const AppStory = Template.bind({});

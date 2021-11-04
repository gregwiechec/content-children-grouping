import React from "react";
import { ComponentStory, ComponentMeta } from "@storybook/react";
// @ts-ignore
import { MemoryRouter, Route } from "react-router-dom";
import { EditConfiguration, EditConfigurationProps } from "./edit-configuration";
import "../../App.scss";
import { action } from "@storybook/addon-actions";
import ServerSettingsContext from "../../server-settings";
import { getServerSettings } from "../../fake-data/fake-server-settings";

interface EditConfigurationStorybookProps extends EditConfigurationProps {
    router: boolean;
    virtualContainersEnabled: boolean;
    physicalContainersEnabled: boolean;
}

const EditConfigurationStorybook = (props: EditConfigurationStorybookProps) => {
    var overridden: any = {};
    if (props.router === false) {
        overridden.routerEnabled = false;
    }
    if (props.virtualContainersEnabled === false) {
        overridden.virtualContainersEnabled = false;
    }
    if (props.physicalContainersEnabled === false) {
        overridden.physicalContainersEnabled = false;
    }
    const serverSettings = getServerSettings(overridden);

    return (
        <ServerSettingsContext.Provider value={serverSettings}>
            <EditConfiguration {...props} onSaveSuccess={action("onSave")} />
        </ServerSettingsContext.Provider>
    );
};

export default {
  title: "Edit Dialog",
  component: EditConfigurationStorybook,
  decorators: [
    (Story) => (
      <MemoryRouter initialEntries={["/path/12345"]}>
        <Route path="/path/:editContentLink">
          <Story />
        </Route>
      </MemoryRouter>
    )
  ]
} as ComponentMeta<typeof EditConfigurationStorybook>;

const Template: ComponentStory<typeof EditConfigurationStorybook> = (args) => <EditConfigurationStorybook {...args} />

export const Default = Template.bind({});

export const WithNoRouter = Template.bind({});
WithNoRouter.args = {
    router: false
}

export const WithNoVirtualContainers = Template.bind({});
WithNoVirtualContainers.args = {
    virtualContainersEnabled: false
}

export const NoPhysicalsContainers = Template.bind({});
NoPhysicalsContainers.args = {
    physicalContainersEnabled: false
}

import React from "react";
import { ComponentStory, ComponentMeta } from "@storybook/react";
import "../../App.scss";
import ServerSettingsContext from "../../server-settings";
import { getServerSettings } from "../../fake-data/fake-server-settings";
import {PluginInfo} from "./plugin-info";

interface PluginInfoStorybookStorybookProps {
    virtualContainersEnabled: boolean;
}

const PluginInfoStorybook = (props: PluginInfoStorybookStorybookProps) => {
    let overridden: any = {};
    if (props.virtualContainersEnabled === false) {
        overridden.virtualContainersEnabled = false;
    }
    const serverSettings = getServerSettings(overridden);

    return (
        <ServerSettingsContext.Provider value={serverSettings}>
            <PluginInfo />
        </ServerSettingsContext.Provider>
    );
};

export default {
  title: "Plugin info",
  component: PluginInfoStorybook,
} as ComponentMeta<typeof PluginInfoStorybook>;

const Template: ComponentStory<typeof PluginInfoStorybook> = (args) => <PluginInfoStorybook {...args} />

export const Default = Template.bind({});

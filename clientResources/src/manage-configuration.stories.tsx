import React from "react";
import { ComponentStory, ComponentMeta } from "@storybook/react";
import { ManageConfiguration } from "./manage-configuration";
import "./App.scss";
import { fakeService } from "./fake-data/fake-service";
import {GroupConfiguration} from "./models/group-configuration";

export default {
  title: "Manage dialog",
  component: ManageConfiguration
} as ComponentMeta<typeof ManageConfiguration>;

const configuration: GroupConfiguration = {
  contentLink: "5",
  fromCode: false,
  routingEnabled: true,
  groupLevelConfigurations: [],
  containerTypeName: ""
};

const Template: ComponentStory<typeof ManageConfiguration> = (args) => (
  <ManageConfiguration structureUpdateEnabled={args.structureUpdateEnabled} configuration={configuration} dataService={fakeService} onCancel={() => {}} />
);

export const DefaultManageConfigurationDialog = Template.bind({ structureUpdateEnabled: true });
DefaultManageConfigurationDialog.args = {
  structureUpdateEnabled: true
};

export const ReadonlyManageConfigurationDialog = Template.bind({ });
ReadonlyManageConfigurationDialog.args = {
  structureUpdateEnabled: false
}

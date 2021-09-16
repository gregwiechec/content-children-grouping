import React from "react";
import { ComponentStory, ComponentMeta } from "@storybook/react";
import { ManageConfigurationDialog } from "./manage-configuration-dialog";
import "./App.scss";
import { fakeService } from "./fake-data/fake-service";
import {GroupConfiguration} from "./models/Groupconfiguration";

export default {
  title: "Manage dialog",
  component: ManageConfigurationDialog
} as ComponentMeta<typeof ManageConfigurationDialog>;

const configuration: GroupConfiguration = {
  contentLink: "5",
  routingEnabled: true,
  groupLevelConfigurations: [],
  containerTypeName: ""
};

const Template: ComponentStory<typeof ManageConfigurationDialog> = (args) => (
  <ManageConfigurationDialog structureUpdateEnabled={args.structureUpdateEnabled} configuration={configuration} dataService={fakeService} onCancel={() => {}} />
);

export const DefaultManageConfigurationDialog = Template.bind({ structureUpdateEnabled: true });
DefaultManageConfigurationDialog.args = {
  structureUpdateEnabled: true
};

export const ReadonlyManageConfigurationDialog = Template.bind({ });
ReadonlyManageConfigurationDialog.args = {
  structureUpdateEnabled: false
}

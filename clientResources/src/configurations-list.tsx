import React from "react";
import { Button, Table } from "optimizely-oui";
// @ts-ignore
import Icon from "react-oui-icons";
import { GroupConfiguration } from "./models/Groupconfiguration";
import "optimizely-oui/dist/styles.css";

interface ConfigurationsListProps {
  items: GroupConfiguration[];
}

export const ConfigurationsList = ({ items }: ConfigurationsListProps) => {
  return (
    <Table density="loose">
      <Table.THead>
        <Table.TR>
          <Table.TH>Content Link</Table.TH>
          <Table.TH>Container type</Table.TH>
          <Table.TH>Router enabled</Table.TH>
          <Table.TH>Generator</Table.TH>
          <Table.TH>Delete</Table.TH>
        </Table.TR>
      </Table.THead>
      <Table.TBody>
        {(items || []).map((x) => (
          <Table.TR>
            <Table.TD>{x.contentLink}</Table.TD>
            <Table.TD width="20%">{x.containerTypeName}</Table.TD>
            <Table.TD>{x.routingEnabled && <Icon name="check" />}</Table.TD>
            <Table.TD>{(x.groupLevelConfigurations || []).join(", ")}</Table.TD>
            <Table.TD>
              <Button style="plain" size="narrow" leftIcon="projects">Edit</Button>
              <Button style="plain" size="narrow" leftIcon="ban">Delete</Button>
            </Table.TD>
          </Table.TR>
        ))}
      </Table.TBody>
    </Table>
  );
};

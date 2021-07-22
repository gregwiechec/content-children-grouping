import React, { useState } from "react";
import { Button, Table } from "optimizely-oui";
// @ts-ignore
import Icon from "react-oui-icons";
import { GroupConfiguration } from "./models/Groupconfiguration";
import "optimizely-oui/dist/styles.css";
import { ConfirmDialog } from "./confirm-dialog";

interface ConfigurationsListProps {
  items: GroupConfiguration[];
  onEdit: (item: GroupConfiguration) => void;
  onDelete: (item: GroupConfiguration) => void;
}

export const ConfigurationsList = ({ items, onEdit, onDelete }: ConfigurationsListProps) => {
  const [itemToDelete, setItemToDelete] = useState<GroupConfiguration | null>(null);

  const onConfigurationDelete = () => {
      if (!itemToDelete) {
          return;
      }
      onDelete(itemToDelete);
      setItemToDelete(null);
  };

  return (
    <>
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
            <Table.TR key={x.contentLink}>
              <Table.TD>{x.contentLink}</Table.TD>
              <Table.TD width="20%">{x.containerTypeName}</Table.TD>
              <Table.TD>{x.routingEnabled && <Icon name="check" />}</Table.TD>
              <Table.TD>{(x.groupLevelConfigurations || []).join(", ")}</Table.TD>
              <Table.TD>
                <Button style="plain" size="narrow" leftIcon="projects" onClick={() => onEdit(x)}>
                  Edit
                </Button>
                <Button style="plain" size="narrow" leftIcon="ban" onClick={() => setItemToDelete(x)}>
                  Delete
                </Button>
              </Table.TD>
            </Table.TR>
          ))}
        </Table.TBody>
      </Table>
      <ConfirmDialog
        open={!!itemToDelete}
        message={"Do you want to delete configuration"}
        onCancel={() => setItemToDelete(null)}
        onConfirm={onConfigurationDelete}
      />
    </>
  );
};

import React, { useState } from "react";
import { Button, Table } from "optimizely-oui";
// @ts-ignore
import Icon from "react-oui-icons";
import { GroupConfiguration } from "./models/Groupconfiguration";
import "optimizely-oui/dist/styles.css";
import { ConfirmDialog } from "./confirm-dialog";

//TODO: show notification when database configuration is not enabled

interface ConfigurationsListProps {
  items: GroupConfiguration[];
  onEdit: (item: GroupConfiguration) => void;
  onManage: (item: GroupConfiguration) => void;
  onDelete: (item: GroupConfiguration) => void;
}

export const ConfigurationsList = ({ items, onEdit, onManage, onDelete }: ConfigurationsListProps) => {
  const [itemToDelete, setItemToDelete] = useState<GroupConfiguration | null>(null);

  if (!items) {
    items = [];
  }

  const onConfigurationDelete = () => {
    if (!itemToDelete) {
      return;
    }
    onDelete(itemToDelete);
    setItemToDelete(null);
  };
  return (
    <>
      <Table density="loose" className="configuration-table">
        <Table.THead>
          <Table.TR>
            <Table.TH width={100}>Content Link</Table.TH>
            <Table.TH width={90}>From code</Table.TH>
            <Table.TH>Container type</Table.TH>
            <Table.TH width={90}>Router</Table.TH>
            <Table.TH width={200}>Generator</Table.TH>
            <Table.TH width={170}>&nbsp;</Table.TH>
          </Table.TR>
        </Table.THead>
        <Table.TBody>
          {items.map((x) => (
            <Table.TR key={x.contentLink}>
              <Table.TD>{x.contentLink}</Table.TD>
              <Table.TD>{x.fromCode && <Icon name="check" />}</Table.TD>
              <Table.TD>
                {x.containerTypeName}
                {!x.containerTypeName && <span style={{ fontStyle: "italic" }}>[default]</span>}
              </Table.TD>
              <Table.TD className="centered">{x.routingEnabled && <Icon name="check" />}</Table.TD>
              <Table.TD>{(x.groupLevelConfigurations || []).join(" => ")}</Table.TD>
              <Table.TD>
                {!x.fromCode && (
                  <Button style="plain" size="narrow" leftIcon="projects" onClick={() => onEdit(x)}>
                    Edit
                  </Button>
                )}
                <Button style="plain" size="narrow" leftIcon="settings" onClick={() => onManage(x)}>
                  Manage
                </Button>
                {!x.fromCode && (
                  <Button style="plain" size="narrow" leftIcon="ban" onClick={() => setItemToDelete(x)}>
                    Delete
                  </Button>
                )}
              </Table.TD>
            </Table.TR>
          ))}

          {items.length === 0 && (
            <Table.TR>
              <Table.TD className="empty-config-table" colSpan={5}>
                No configuration
              </Table.TD>
            </Table.TR>
          )}
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

import React, { useState } from "react";
import { BlockList, ButtonIcon, Link, OverlayWrapper, Popover, Table } from "optimizely-oui";
// @ts-ignore
import Icon from "react-oui-icons";
import { GroupConfiguration } from "../../models/group-configuration";
import "optimizely-oui/dist/styles.css";
import { ConfirmDialog } from "../../confirm-dialog";
import { useServerSettingsContext } from "../../server-settings";
import { ContentLink } from "../../content-link";

export interface ConfigurationsListProps {
  items: GroupConfiguration[];
  onEdit: (item: GroupConfiguration) => void;
  onDelete: (item: GroupConfiguration) => void;
}

export const ConfigurationsList = ({ items, onEdit, onDelete }: ConfigurationsListProps) => {
  const {
    options: { databaseConfigurationsEnabled }
  } = useServerSettingsContext();

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
      <Table density="loose" className="configuration-table plugin-grid">
        <Table.THead>
          <Table.TR>
            <Table.TH width="120px">Content Link</Table.TH>
            <Table.TH width="80px">From code</Table.TH>
            <Table.TH width="200px">Generator</Table.TH>
            <Table.TH width="200px">&nbsp;</Table.TH>
          </Table.TR>
        </Table.THead>
        <Table.TBody>
          {items.map((x) => (
            <Table.TR key={x.contentLink}>
              <Table.TD>
                <ContentLink value={x.contentLink} contentExists={x.contentExists} />
              </Table.TD>
              <Table.TD>{x.fromCode && <Icon name="check" />}</Table.TD>
              <Table.TD>{(x.groupLevelConfigurations || []).map((x) => x.name).join(" => ")}</Table.TD>
              <Table.TD className="menu-cell">
                {/*
                <OverlayWrapper
                  behavior="click"
                  horizontalAttachment="left"
                  horizontalTargetAttachment="left"
                  verticalAttachment="top"
                  verticalTargetAttachment="bottom"
                  shouldHideOnClick={true}
                  overlay={
                    <Popover>
                      <BlockList hasBorder={false}>
                        <BlockList.Item onClick={() => onEdit(x)}>
                          <Link leftIcon="projects">Edit</Link>
                        </BlockList.Item>
                        {!x.fromCode && databaseConfigurationsEnabled && (
                          <BlockList.Item onClick={() => setItemToDelete(x)}>
                            <Link leftIcon="ban">Delete</Link>
                          </BlockList.Item>
                        )}
                      </BlockList>
                    </Popover>
                  }
                >
                  <ButtonIcon title="" iconName="ellipsis" style="plain" />
                </OverlayWrapper>
*/}
                {/*//TODO: context menu is not working*/}
                <Link leftIcon="projects" onClick={() => onEdit(x)}>
                  Edit
                </Link>
                &nbsp;
                {!x.fromCode && databaseConfigurationsEnabled && (
                  <Link onClick={() => setItemToDelete(x)} leftIcon="ban">
                    Delete
                  </Link>
                )}
              </Table.TD>
            </Table.TR>
          ))}

          {items.length === 0 && (
            <Table.TR>
              <Table.TD className="empty-config-table" colSpan={7}>
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

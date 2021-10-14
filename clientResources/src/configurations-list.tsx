import React, { useState } from "react";
import { BlockList, ButtonIcon, Link, OverlayWrapper, Popover, Table } from "optimizely-oui";
// @ts-ignore
import Icon from "react-oui-icons";
import { GroupConfiguration } from "./models/Groupconfiguration";
import "optimizely-oui/dist/styles.css";
import { ConfirmDialog } from "./confirm-dialog";
import { useServerSettingsContext } from "./server-settings";
import { ContentLink } from "./ContentLink";

interface ConfigurationsListProps {
  items: GroupConfiguration[];
  onEdit: (item: GroupConfiguration) => void;
  onManage: (item: GroupConfiguration) => void;
  onDelete: (item: GroupConfiguration) => void;
}

export const ConfigurationsList = ({ items, onEdit, onManage, onDelete }: ConfigurationsListProps) => {
  const {
    defaultContainerType,
    options: { databaseConfigurationsEnabled, virtualContainersEnabled }
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
      <Table density="loose" className="configuration-table">
        <Table.THead>
          <Table.TR>
            <Table.TH width={100}>Content Link</Table.TH>
            <Table.TH width={90}>From code</Table.TH>
            {virtualContainersEnabled && <Table.TH width={90}>Is virtual</Table.TH>}
            <Table.TH width={90}>Router</Table.TH>
            <Table.TH>Container type</Table.TH>
            <Table.TH width={200}>Generator</Table.TH>
            <Table.TH width={200}>&nbsp;</Table.TH>
          </Table.TR>
        </Table.THead>
        <Table.TBody>
          {items.map((x) => (
            <Table.TR key={x.contentLink}>
              <Table.TD>
                <ContentLink value={x.contentLink} contentExists={x.contentExists} />
              </Table.TD>
              <Table.TD>{x.fromCode && <Icon name="check" />}</Table.TD>
              {virtualContainersEnabled && <Table.TD>{x.isVirtualContainer && <Icon name="check" />}</Table.TD>}
              <Table.TD>{x.routingEnabled && <Icon name="check" />}</Table.TD>
              <Table.TD>
                {x.containerTypeName}
                {!x.containerTypeName && (
                  <span style={{ fontStyle: "italic" }} title={defaultContainerType}>
                    [default]
                  </span>
                )}
              </Table.TD>
              <Table.TD>{(x.groupLevelConfigurations || []).join(" => ")}</Table.TD>
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
                        <BlockList.Item onClick={() => onManage(x)}>
                          <Link leftIcon="settings">Manage</Link>
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
                <Link leftIcon="settings" onClick={() => onManage(x)}>
                  Manage
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

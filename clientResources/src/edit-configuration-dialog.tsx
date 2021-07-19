import React, { useState } from "react";
import { GroupConfiguration } from "./models/Groupconfiguration";
import { BlockList, Button, ButtonIcon, Checkbox, DialogNew, GridCell, Input, Label, Select } from "optimizely-oui";

interface EditConfigurationDialogProps {
  onSave: (configuration: GroupConfiguration) => void;
  onCancel: () => void;
  configuration?: GroupConfiguration;
  availableNameGenerators: string[];
}

export const EditConfigurationDialog = ({
  onSave,
  onCancel,
  configuration,
  availableNameGenerators
}: EditConfigurationDialogProps) => {
  const [contentLink, setContentLink] = useState(configuration?.contentLink || "");
  const [containerTypeName, setContainerTypeName] = useState(configuration?.containerTypeName || "");
  const [isRoutingEnabled, setIsRoutingEnabled] = useState(configuration?.routingEnabled || false);
  const [generators, setGenerators] = useState(configuration?.groupLevelConfigurations || []);

  const onAddGenerator = (item: GroupConfiguration) => {
    const updatedList = [...generators, availableNameGenerators[0]];
    setGenerators(updatedList);
  };

  const onRemoveGenerator = (index: number) => {
    let updatedList = [...generators];
    updatedList.splice(index, 1);
    setGenerators(updatedList);
  };

  const onGeneratorValueChange = (index: number, value: string) => {
    let updatedList = [...generators];
    updatedList[index] = value;
    setGenerators(updatedList);
  };

  const onDialogSave = () => {
    onSave({
      contentLink: contentLink,
      containerTypeName: containerTypeName,
      routingEnabled: isRoutingEnabled,
      groupLevelConfigurations: generators
    });
  };

  return (
    <DialogNew
      title="Configuration"
      hasCloseButton={true}
      hasOverlay={true}
      onClose={onCancel}
      footerButtonList={[
        <Button style="plain" key={0} onClick={onCancel}>
          Cancel
        </Button>,
        <Button style="highlight" key={1} onClick={onDialogSave}>
          Save
        </Button>
      ]}
    >
      <Input
        defaultValue={configuration?.contentLink}
        displayError={false}
        type="number"
        isOptional={false}
        label="Container Content link"
        maxLength={5}
        className="content-link"
        min={1}
        onChange={e => setContentLink(e.target.value)}
        value={contentLink}
      />
      <Input defaultValue={configuration?.containerTypeName} type="text" label="Container type name" />
      <Checkbox defaultChecked={configuration?.routingEnabled} label="Router enabled" />
      <Label>Name generators</Label>
      <BlockList hasBorder={false} className="configuration-item">
        {generators.map((x, index) => (
          <BlockList.Item key={x + "_" + index}>
            <Select
              className="configuration-generator-select"
              isOptional={false}
              onChange={(value) => onGeneratorValueChange(index, value.target.value)}
            >
              {availableNameGenerators.map((generator) => (
                <option key={generator} value={generator} selected={generator === x}>
                  {generator}
                </option>
              ))}
            </Select>
            {index > 0 && (
              <ButtonIcon
                className="remove-button"
                iconName="close"
                isDisabled={false}
                onClick={() => onRemoveGenerator(index)}
                size="small"
                style="outline"
                title="Close Dialog"
              />
            )}
          </BlockList.Item>
        ))}
      </BlockList>
      <Button style="outline" size="narrow" leftIcon="add" onClick={onAddGenerator}>
        Add generator
      </Button>
    </DialogNew>
  );
};

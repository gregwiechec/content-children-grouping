import React, { useEffect, useState } from "react";
import { GroupConfiguration } from "./models/Groupconfiguration";
import { Attention, BlockList, Button, ButtonIcon, Checkbox, DialogNew, Input, Label, Select } from "optimizely-oui";

interface EditConfigurationDialogProps {
  onSave: (configuration: GroupConfiguration) => void;
  onCancel: () => void;
  configuration?: GroupConfiguration;
  availableNameGenerators: string[];
  validationMessage: string;
}
//TODO: show type format
//TODO: one generator is mandatory

export const EditConfigurationDialog = ({
  onSave,
  onCancel,
  configuration,
  availableNameGenerators,
  validationMessage
}: EditConfigurationDialogProps) => {
  const [contentLink, setContentLink] = useState(configuration?.contentLink || "");
  const [containerTypeName, setContainerTypeName] = useState(configuration?.containerTypeName || "");
  const [isRoutingEnabled, setIsRoutingEnabled] = useState(configuration?.routingEnabled || false);
  const [generators, setGenerators] = useState(configuration?.groupLevelConfigurations || []);

  useEffect(() => {
    setContentLink(configuration?.contentLink || "");
    setContainerTypeName(configuration?.containerTypeName || "");
    setIsRoutingEnabled(configuration?.routingEnabled || false);
    setGenerators(configuration?.groupLevelConfigurations || []);
  }, [configuration]);

  const onAddGenerator = () => {
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

  const isValid = (): boolean => {
    if (!contentLink || contentLink.trim() === "") {
      return false;
    }

    if (!generators || generators.length === 0) {
      return false;
    }

    return true;
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
      className="configuration-item"
      hasCloseButton={true}
      hasOverlay={true}
      onClose={onCancel}
      footerButtonList={[
        <Button style="plain" key={0} onClick={onCancel}>
          Cancel
        </Button>,
        <Button isDisabled={!isValid()} style="highlight" key={1} onClick={onDialogSave}>
          Save
        </Button>
      ]}
    >
      {validationMessage && (
        <Attention alignment="center" type="bad-news">
          {validationMessage}
        </Attention>
      )}

      <Input
        displayError={false}
        type="number"
        isOptional={false}
        label="Container Content link"
        maxLength={5}
        className="content-link"
        min={1}
        onChange={(e) => setContentLink(e.target.value)}
        value={contentLink}
        isRequired
      />
      <br/>
      <Input
        type="text"
        label="Container type name"
        note="Type format: [Full type name, Assembly Name]"
        value={containerTypeName}
        onChange={(e) => setContainerTypeName(e.target.value)}
      />
      <br/>
      <Checkbox
        label="Router enabled"
        checked={isRoutingEnabled}
        onChange={(e) => setIsRoutingEnabled(e.target.checked)}
      />
      <br/>
      <Label>Name generators *</Label>
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

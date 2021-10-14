import React, { useState } from "react";
import { BlockList, Button, ButtonIcon, Select } from "optimizely-oui";
import { GeneratorConfiguration } from "../models/group-configuration";
import { GeneratorSettingsDialog } from "./generator-settings-dialog";

interface GeneratorsListProps {
  availableNameGenerators: string[];
  generators: GeneratorConfiguration[];
  isReadonly: boolean;
  onGeneratorValueChange: (index: number, value: string) => void;
  onRemoveGenerator: (index: number) => void;
  onSettingsChanged: (index: number, settings: Record<string, string>) => void;
  onAddGenerator: () => void;
  databaseConfigurationsEnabled: boolean;
}

export const GeneratorsList = ({
  availableNameGenerators,
  generators,
  isReadonly,
  onGeneratorValueChange,
  onRemoveGenerator,
  onAddGenerator,
  onSettingsChanged,
  databaseConfigurationsEnabled
}: GeneratorsListProps) => {
  const [editedGenerator, setEditedGenerator] = useState<GeneratorConfiguration | null>(null);

  const settingsEnabled = (generator: GeneratorConfiguration) => {
    const name = generator?.name?.toLowerCase() || "";
    return !(name === "name" || name === "create date");
  };

  const onSaveGeneratorSettings = (generatorSettings: Record<string, string>) => {
    if (!editedGenerator) {
      return;
    }
    let index = generators.indexOf(editedGenerator);
    onSettingsChanged(index, generatorSettings);
    setEditedGenerator(null);
  };

  return (
    <>
      <BlockList hasBorder={false} className="configuration-item">
        {generators.map((x, index) => (
          <BlockList.Item key={x + "_" + index}>
            {!isReadonly && (
              <>
                <Select
                  className="configuration-generator-select"
                  isOptional={false}
                  onChange={(value) => onGeneratorValueChange(index, value.target.value)}
                >
                  {availableNameGenerators.map((generator) => (
                    <option key={generator} value={generator} selected={generator === x.name}>
                      {generator}
                    </option>
                  ))}
                </Select>
                <ButtonIcon
                  className="remove-button"
                  iconName="settings"
                  onClick={() => setEditedGenerator(x)}
                  isDisabled={settingsEnabled(x)}
                  size="small"
                  style="outline"
                  title="Change settings"
                />
                <ButtonIcon
                  className="remove-button"
                  iconName="close"
                  isDisabled={index === 0}
                  onClick={() => onRemoveGenerator(index)}
                  size="small"
                  style="outline"
                  title="Close Dialog"
                />
              </>
            )}

            {isReadonly && <span key={x.name + "_" + index}>{x.name}</span>}
          </BlockList.Item>
        ))}
      </BlockList>
      {!isReadonly && (
        <Button
          style="outline"
          size="narrow"
          leftIcon="add"
          onClick={onAddGenerator}
          isDisabled={databaseConfigurationsEnabled && (availableNameGenerators?.length || 0) === 0}
        >
          Add generator
        </Button>
      )}
      {editedGenerator && (
        <GeneratorSettingsDialog
          generator={editedGenerator}
          onClose={() => setEditedGenerator(null)}
          onSave={(generatorSettings) => onSaveGeneratorSettings(generatorSettings)}
        />
      )}
    </>
  );
};

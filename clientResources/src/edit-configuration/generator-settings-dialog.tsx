import React, { useEffect, useState } from "react";
import { Button, DialogNew, Fieldset, Input } from "optimizely-oui";
import { GeneratorConfiguration } from "../models/group-configuration";

interface GeneratorSettingDialogProps {
  generator: GeneratorConfiguration | null;
  onClose: () => void;
  onSave: (value: Record<string, string>) => void;
}

export const GeneratorSettingsDialog = ({ generator, onClose, onSave }: GeneratorSettingDialogProps) => {
  const [currentValue, setCurrentValue] = useState<Record<string, string>>({});

  useEffect(() => {
    setCurrentValue(getDefaultGeneratorSettings(generator));
  }, [generator]);

  const updateValue = (key: string, value: string) => {
    const updated = Object.assign({}, currentValue);
    updated[key] = value;
    setCurrentValue(updated);
  };

  return (
    <DialogNew
      title="Generator settings"
      className="generator-settings-dialog"
      hasCloseButton={true}
      hasOverlay={true}
      onClose={onClose}
      footerButtonList={[
        <Button style="plain" key={0} onClick={onClose}>
          Cancel
        </Button>,
        <Button style="highlight" key={1} onClick={() => onSave(currentValue)}>
          Save
        </Button>
      ]}
    >
      <Fieldset>
        {Object.keys(currentValue || {}).map((x) => {
          return (
            <Input key={x} id={x} label={x} value={currentValue[x]} type="text" onChange={(v) => updateValue(x, v.target.value)} />
          );
        })}
      </Fieldset>
    </DialogNew>
  );
};

const getDefaultGeneratorSettings = (generator: GeneratorConfiguration | null): Record<string, string> => {
  if (!generator?.name) {
    return {};
  }
  let availableSettings: string[];
  switch (generator.name.toLowerCase()) {
    case "name": {
      availableSettings = ["startIndex", "countLetters", "defaultName"];
      break;
    }
    case "create date": {
      availableSettings = ["dateFormat", "defaultValue"];
      break;
    }
    default: {
      availableSettings = [];
      break;
    }
  }
  let result: Record<string, string> = {};
  availableSettings.forEach((x) => {
    result[x] = (generator.settings || {})[x];
  });
  return result;
};

//TODO: support numbers for inputs
//TODO: change size of inputs
//TODO: support default values when crating generators

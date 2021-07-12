import React from "react";
import "optimizely-oui/dist/styles.css";
import { GroupConfiguration } from "./models/Groupconfiguration";
import { Button, Checkbox, GridContainer, Grid, GridCell, Input, Label, Select, ButtonIcon } from "optimizely-oui";

interface ConfigurationItemProps {
  configuration: GroupConfiguration;
  availableNameGenerators: string[];
  onAddGenerator: () => void;
  onRemoveGenerator: (index: number) => void;
  onGeneratorValueChange: (index: number, value: string) => void;
}

export const ConfigurationItem = ({
  configuration,
  availableNameGenerators,
  onAddGenerator,
  onRemoveGenerator,
  onGeneratorValueChange
}: ConfigurationItemProps) => {
  if (!availableNameGenerators) {
    availableNameGenerators = [];
  }

  return (
    <GridContainer className="configuration-item">
      <Grid>
        <GridCell className="config-grid-cell" large={12} medium={8} small={4}>
          <Input
            className="content-link"
            defaultValue={configuration.contentLink}
            displayError={false}
            isFilter={false}
            type="number"
            isOptional={false}
            label="Content link"
            maxLength={5}
            min={1}
            placeholder="Container content link"
          />
          <Input defaultValue={configuration.containerTypeName} type="text" label="Container type name" />
          <Checkbox className="input1" defaultChecked={configuration.routingEnabled} label="Router enabled" />
        </GridCell>
        <GridCell className="config-grid-cell" large={12} medium={8} small={4}>
          <Label>Name generators</Label>
          {configuration.groupLevelConfigurations.map((x, index) => (
            <>
              <Select
                className="configuration-generator-select"
                isOptional={false}
                name="zoo"
                id="zoo"
                onChange={(value) => onGeneratorValueChange(index, value)}
              >
                {availableNameGenerators.map((generator) => (
                  <option key={generator} value={generator}>
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
            </>
          ))}
          <Button className="add-button" style="highlight" size="narrow" leftIcon="add" onClick={onAddGenerator}>
            Add generator
          </Button>
        </GridCell>
      </Grid>
    </GridContainer>
  );
};

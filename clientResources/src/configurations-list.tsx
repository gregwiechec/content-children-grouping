import React, { useState } from "react";
import { BlockList } from "optimizely-oui";
import { GroupConfiguration } from "./models/Groupconfiguration";
import { ConfigurationItem } from "./configuration-item";
import "optimizely-oui/dist/styles.css";

interface ConfigurationsListProps {
  items: GroupConfiguration[];
  availableNameGenerators: string[];
  onListChange: (items: GroupConfiguration[]) => void;
}

export const ConfigurationsList = ({ items, availableNameGenerators, onListChange }: ConfigurationsListProps) => {
  const [currentItems, setCurrentItems] = useState(items || []);

  const onAddGenerator = (item: GroupConfiguration) => {
    item.groupLevelConfigurations.push(availableNameGenerators[0]);
    const itemsCopy = [...items];
    onListChange(itemsCopy);
    setCurrentItems(itemsCopy);
  };

  const onRemoveGenerator = (item: GroupConfiguration, index: number) => {
    item.groupLevelConfigurations.splice(index, 1);
    let itemsCopy = [...items];
    onListChange(itemsCopy);
    setCurrentItems(itemsCopy);
  };

  const onGeneratorValueChange = (item: GroupConfiguration, index: number, value: string) => {
    item.groupLevelConfigurations[index] = value;
    let itemsCopy = [...items];
    onListChange(itemsCopy);
    setCurrentItems(itemsCopy);
  };

  return (
    <BlockList hasBorder={false}>
      {currentItems.map((x) => (
        <BlockList.Category>
          <BlockList.Item>
            <ConfigurationItem
              key={x.contentLink}
              availableNameGenerators={availableNameGenerators}
              configuration={x}
              onAddGenerator={() => onAddGenerator(x)}
              onRemoveGenerator={(index) => onRemoveGenerator(x, index)}
              onGeneratorValueChange={(index, value) => onGeneratorValueChange(x, index, value)}
            />
          </BlockList.Item>
        </BlockList.Category>
      ))}
    </BlockList>
  );
};

import React from 'react';
import {BlockList} from 'optimizely-oui';
import {GroupConfiguration} from "./models/Groupconfiguration";
import {ConfigurationItem} from "./configuration-item";
import "optimizely-oui/dist/styles.css";

interface ConfigurationsListProps {
    items: GroupConfiguration[];
}

export const ConfigurationsList = ({items}: ConfigurationsListProps) => {
    if (!items) {
        items = [];
    }

    return (
        <BlockList hasBorder={false}>
            {items.map(x => (
                <BlockList.Category>
                    <BlockList.Item>
                        <ConfigurationItem key={x.contentLink} configuration={x}/>
                    </BlockList.Item>
                </BlockList.Category>
            ))}
        </BlockList>
    );
}

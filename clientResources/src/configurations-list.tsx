import React from 'react';
import { BlockList } from 'optimizely-oui';

import "optimizely-oui/dist/styles.css";

interface ConfigurationsListProps {
    items: any[];
}

export const ConfigurationsList = ({ items }: ConfigurationsListProps) => {
    return (
        <BlockList hasBorder={ true } >
            <BlockList.Category header="Header">
                <BlockList.Item>
                    Home
                </BlockList.Item>
                <BlockList.Item>
                    Shopping Cart
                </BlockList.Item>
                <BlockList.Item>
                    Order Confirmation
                </BlockList.Item>
            </BlockList.Category>
        </BlockList>
    );
}

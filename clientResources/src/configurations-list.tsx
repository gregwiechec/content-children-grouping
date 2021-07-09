import React from 'react';

interface ConfigurationsListProps {
    items: any[];
}

export const ConfigurationsList = ({ items }: ConfigurationsListProps) => {
    return (
        <ul>
            {items.map(x => <li>test</li>)}
        </ul>
    );
}

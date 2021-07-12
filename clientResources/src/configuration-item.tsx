import React from 'react';
import "optimizely-oui/dist/styles.css";
import {GroupConfiguration} from "./models/Groupconfiguration";
import {Checkbox, GridContainer, Grid, GridCell, Input, Label, Select} from 'optimizely-oui';

interface ConfigurationItemProps {
    configuration: GroupConfiguration;
}

export const ConfigurationItem = ({configuration}: ConfigurationItemProps) => {
    return <GridContainer className="configuration-item">
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
                <Input
                    defaultValue={configuration.containerTypeName}
                    type="text"
                    label="Container type name"
                />
                <Checkbox
                    className="input1"
                    defaultChecked={configuration.routingEnabled}
                    label="Router enabled"
                />
            </GridCell>
            <GridCell className="config-grid-cell" large={12} medium={8} small={4}>
                <Label>Name generators</Label>
                <Select
                    className="configuration-generator-select"
                    isOptional={false}
                    name="zoo"
                    id="zoo"
                    onChange={() => alert(1)}
                >
                    <option value="one ddsada dasdasda dadasda dsadsa dsada dsada">Name dadsadas dsadsadasd dsadasda dsada</option>
                    <option value="two">Date</option>
                </Select>
            </GridCell>
        </Grid>
    </GridContainer>
};

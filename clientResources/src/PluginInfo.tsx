import React from "react";
// @ts-ignore
import { Link } from "react-router-dom";
import { useServerSettingsContext } from "./server-settings";
import { Grid, GridCell, GridContainer, Table } from "optimizely-oui";

export const PluginInfo = () => {
  const serverSettings = useServerSettingsContext();

  const options = Object.keys(serverSettings.options);

  return (
    <GridContainer className="configuration-item">
      <Grid>
        <GridCell large={12} medium={8} small={4}>
          <h2>Plugin information</h2>
          <Table density="loose">
            <Table.THead>
              <Table.TR>
                <Table.TH>Parameter</Table.TH>
                <Table.TH>Value</Table.TH>
              </Table.TR>
            </Table.THead>
            <Table.TBody>
              {options.map((x) => (
                <Table.TR key={x}>
                  <Table.TD>{x}</Table.TD>
                  <Table.TD>{(serverSettings.options as any)[x]?.toString()}</Table.TD>
                </Table.TR>
              ))}
            </Table.TBody>
          </Table>
          <Link to="/">Back</Link>
        </GridCell>
      </Grid>
    </GridContainer>
  );
};

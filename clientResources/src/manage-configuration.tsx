import React, { useEffect, useState } from "react";
// @ts-ignore
import { useHistory, useParams } from "react-router-dom";
import { GroupConfiguration } from "./models/group-configuration";
import { Attention, Button, Grid, GridCell, GridContainer } from "optimizely-oui";
import { useDataServiceContext } from "./data-service";
import {useServerSettingsContext} from "./server-settings";

export const ManageConfiguration = () => {
  const { contentLink } = useParams();
  const history = useHistory();
  const dataService = useDataServiceContext();
  const { options } = useServerSettingsContext();

  const [configuration, setConfiguration] = useState<GroupConfiguration | null>(null);
  const [inProgress, setIsInProgress] = useState(false);
  const [message, setMessage] = useState("");

  useEffect(() => {
    setMessage("");
  }, [configuration]);

  useEffect(() => {
    if (contentLink) {
      dataService?.get(contentLink).then((result: GroupConfiguration) => {
        if (!result) {
          return;
        }
        setConfiguration(result);
      });
    }
  }, [contentLink, dataService]);

  const onClearStructureClick = () => {
    setIsInProgress(true);
    setMessage("");
    dataService.clearContainers(configuration?.contentLink || "").then((result) => {
      setIsInProgress(false);
      setMessage(result?.data || "");
    });
  };

  const onCloseClock = () => {
    if (inProgress) {
      return;
    }
    history.push("/");
  };

  return (
    <GridContainer>
      <Grid>
        <GridCell large={12} medium={8} small={4}>
          <h2>Manage</h2>
        </GridCell>
      </Grid>

      <Grid>
        <GridCell large={12}>
          <h5>Clear structure</h5>
          {!options.structureUpdateEnabled && (
            <>
              {configuration?.isVirtualContainer &&
                <Attention type="warning">Clear structure is not available for virtual containers</Attention>
                }
              {!!message && <Attention>{message}</Attention>}
              <p>
                Remove container structure. It will delete all ContainerType contents.
                <br />
                All pages will be added to Configuration container.
              </p>
              <Button isDisabled={inProgress || configuration?.isVirtualContainer} onClick={onClearStructureClick}>
                Clear structure
              </Button>
            </>
          )}
          {options.structureUpdateEnabled && (
            <div>
              Containers cannot be cleared, because <strong>StructureUpdateEnabled</strong> option is enabled in
              options.
              <br />
            </div>
          )}
        </GridCell>
      </Grid>

      <Grid>
        <GridCell large={12} medium={8} small={4}>
          <br />
          <Button style="plain" key={0} onClick={onCloseClock}>
            Back
          </Button>
        </GridCell>
      </Grid>
    </GridContainer>
  );
};

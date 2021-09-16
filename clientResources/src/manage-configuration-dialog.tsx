import React, { useEffect, useState } from "react";
import { GroupConfiguration } from "./models/Groupconfiguration";
import { Attention, Button, DialogNew } from "optimizely-oui";
import { DataService } from "./data-service";

interface EditConfigurationDialogProps {
  onCancel: () => void;
  configuration?: GroupConfiguration;
  dataService: DataService;
  structureUpdateEnabled: boolean;
}

export const ManageConfigurationDialog = ({
  dataService,
  onCancel,
  configuration,
  structureUpdateEnabled
}: EditConfigurationDialogProps) => {
  const [inProgress, setInProgess] = useState(false);
  const [message, setMessage] = useState("");

  useEffect(() => {
    setMessage("");
  }, [configuration]);

  const onClearStructureClick = () => {
    setInProgess(true);
    setMessage("");
    dataService.clearContainers(configuration?.contentLink || "").then((result) => {
      setInProgess(false);
      setMessage(result);
    });
  };

  const onCancelClick = () => {
    if (inProgress) {
      return;
    }
    onCancel();
  };

  return (
    <DialogNew
      title="Manage"
      hasCloseButton={true}
      hasOverlay={true}
      onClose={onCancelClick}
      footerButtonList={[
        <Button style="plain" key={0} onClick={onCancelClick}>
          Close
        </Button>
      ]}
    >
      <h5>Clear structure</h5>
      {!structureUpdateEnabled && (
        <>
          {!!message && <Attention>{message}</Attention>}
          <p>
            Remove container structure. It will delete all ContainerType contents.
            <br />
            All pages will be added to Configuration container.
          </p>
          <Button isDisabled={inProgress} onClick={onClearStructureClick}>
            Clear structure
          </Button>
        </>
      )}
      {structureUpdateEnabled && (
        <div>
          Containers cannot be cleared, because <strong>StructureUpdateEnabled</strong> is enabled in options.
        </div>
      )}
    </DialogNew>
  );
};

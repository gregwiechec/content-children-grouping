import React, { useState } from "react";
import { GroupConfiguration } from "./models/Groupconfiguration";
import { Attention, Button, DialogNew } from "optimizely-oui";
import { DataService } from "./data-service";

interface EditConfigurationDialogProps {
  onCancel: () => void;
  configuration?: GroupConfiguration;
  dataService: DataService;
}
export const ManageConfigurationDialog = ({ dataService, onCancel, configuration }: EditConfigurationDialogProps) => {
  const [inProgress, setInProgess] = useState(false);
  const [message, setMessage] = useState("");

  const onClearStructureClick = () => {
    setInProgess(true);
    setMessage("");
    dataService.clearContainers(configuration?.contentLink).then(result => {
      setInProgess(false);
      setMessage(result);
    });
  };

  return (
    <DialogNew
      title="Manage"
      hasCloseButton={true}
      hasOverlay={true}
      onClose={onCancel}
      footerButtonList={[
        <Button style="plain" key={0} onClick={onCancel}>
          Close
        </Button>
      ]}
    >
      <h5>Clear structure</h5>
      {!!message && <Attention>{message}</Attention>}
      <p>
        Remove container structure. It will delete all ContainerType contents.
        <br />
        All pages will be added to Configuration container.
      </p>
      <Button isDisabled={inProgress} onClick={onClearStructureClick}>
        Clear structure
      </Button>
    </DialogNew>
  );
};

import React from "react";
import { Button, DialogNew } from "optimizely-oui";

interface ConfirmDialogProps {
  open: boolean;
  title?: string;
  message: string;
  onCancel: () => void;
  onConfirm: () => void;
}

export const ConfirmDialog = ({ open, title = "Confirm", message, onConfirm, onCancel }: ConfirmDialogProps) => {
  if (!open) {
    return <></>;
  }

  return (
    <DialogNew
      title={title}
      hasCloseButton={true}
      hasOverlay={true}
      onClose={onCancel}
      footerButtonList={[
        <Button style="plain" key={0} onClick={onCancel}>
          Cancel
        </Button>,
        <Button style="highlight" key={1} onClick={onConfirm}>
          Save
        </Button>
      ]}
    >
      <p>{message}</p>
    </DialogNew>
  );
};

import React from "react";
import { Attention } from "optimizely-oui";

interface MessageProps {
  message?: string;
}

export const Message = ({ message }: MessageProps) => {
  if (!message) {
    return <></>;
  }
  return (
    <Attention alignment="center" type="good-news">
      {message}
    </Attention>
  );
};

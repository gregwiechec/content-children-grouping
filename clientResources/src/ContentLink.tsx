import React from "react";
import { Link } from "optimizely-oui";
import { useServerSettingsContext } from "./server-settings";

interface ContentLinkProps {
  value?: string;
}

export const ContentLink = ({ value }: ContentLinkProps) => {
  const { contentUrl } = useServerSettingsContext();

  if (!contentUrl || !value) {
    return <>{value}</>;
  }

  return (
    <Link href={contentUrl.replace("{contentLink}", value)} newWindow>
      {value}
    </Link>
  );
};

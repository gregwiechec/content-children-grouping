import React from "react";
import { Link } from "optimizely-oui";
// @ts-ignore
import Icon from "react-oui-icons";
import { useServerSettingsContext } from "./server-settings";

interface ContentLinkProps {
  value?: string;
  contentExists?: boolean;
}

export const ContentLink = ({ value, contentExists }: ContentLinkProps) => {
  const { contentUrl } = useServerSettingsContext();

  if (!contentUrl || !value) {
    return <>{value}</>;
  }

  const tooltip = `Content for ${value} doesn't exists`;

  if (typeof contentExists === "boolean" && !contentExists) {
    return (
      <span className="content-link-with-icon" title={tooltip}>
        <Icon name="exclamation" title={tooltip} />
        <span>{value}</span>
      </span>
    );
  }

  return (
    <Link href={contentUrl.replace("{contentLink}", value)}>
      {value}
    </Link>
  );
};

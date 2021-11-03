import { ServerSettings } from "../server-settings";

const getValueOrDefault = (name: string, overriddenValues: any, defaultValue: any) => {
  if (typeof overriddenValues[name] !== "undefined") {
    return overriddenValues[name];
  }
  return defaultValue;
};

export const getServerSettings = (overriddenValues?: any): ServerSettings => {
  let result: ServerSettings = {
    availableNameGenerators: ["Name", "Create Date", "Very long name generator"],
    defaultContainerType: "Alloy.ContainerPage, Alloy",
    contentUrl: "http://google.com/{contentLink}",
    options: {
      customIconsEnabled: false,
      databaseConfigurationsEnabled: true,
      routerEnabled: getValueOrDefault("routerEnabled", overriddenValues, true),
      searchCommandEnabled: false,
      structureUpdateEnabled: false,
      virtualContainersEnabled: getValueOrDefault("virtualContainersEnabled", overriddenValues, true)
    },
    defaultOptions: {
      customIconsEnabled: true,
      databaseConfigurationsEnabled: true,
      routerEnabled: true,
      searchCommandEnabled: true,
      structureUpdateEnabled: true,
      virtualContainersEnabled: true
    }
  };

  if (overriddenValues) {
  }

  return result;
};

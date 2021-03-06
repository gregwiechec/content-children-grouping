import { createContext, useContext } from "react";

export type ContentChildrenGroupingOptions = {
    readonly databaseConfigurationsEnabled: boolean;
    readonly customIconsEnabled: boolean;
    readonly searchCommandEnabled: boolean;
    readonly virtualContainersEnabled: boolean;
}

export type ServerSettings = {
    readonly availableNameGenerators: string[];
    readonly contentUrl: string;
    readonly options: ContentChildrenGroupingOptions
    readonly defaultOptions: ContentChildrenGroupingOptions
}

const defaultSettings: ServerSettings = {
    availableNameGenerators: [],
    contentUrl: "",
    options: {
        customIconsEnabled: false,
        databaseConfigurationsEnabled: false,
        searchCommandEnabled: false,
        virtualContainersEnabled: false
    },
    defaultOptions: {
        customIconsEnabled: false,
        databaseConfigurationsEnabled: false,
        searchCommandEnabled: false,
        virtualContainersEnabled: false
    }
};

const ServerSettingsContext = createContext<ServerSettings>(defaultSettings);

export default ServerSettingsContext;

export const useServerSettingsContext = (): ServerSettings => {
    const serverSettingsContext = useContext(ServerSettingsContext);
    if (!serverSettingsContext) {
        throw new Error('serverSettingsContext must be used within the ServerSettingsContext.Provider');
    }
    return serverSettingsContext;
};

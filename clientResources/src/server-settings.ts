import { createContext, useContext } from "react";

export type ContentChildrenGroupingOptions = {
    readonly routerEnabled: boolean;
    readonly structureUpdateEnabled: boolean;
    readonly databaseConfigurationsEnabled: boolean;
    readonly customIconsEnabled: boolean;
    readonly searchCommandEnabled: boolean;
    readonly virtualContainersEnabled: boolean;
}

export type ServerSettings = {
    readonly availableNameGenerators: string[];
    readonly contentUrl: string;
    readonly defaultContainerType: string;
    readonly options: ContentChildrenGroupingOptions
}


const defaultSettings: ServerSettings = {
    availableNameGenerators: [],
    defaultContainerType: "",
    contentUrl: "",
    options: {
        customIconsEnabled: false,
        databaseConfigurationsEnabled: false,
        routerEnabled: false,
        searchCommandEnabled: false,
        structureUpdateEnabled: false
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

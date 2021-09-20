import { createContext, useContext } from "react";

export type ServerSettings = {
    readonly structureUpdateEnabled: boolean;
    readonly availableNameGenerators: string[];
    readonly databaseConfigurationsEnabled: boolean;
    readonly contentUrl: string;
    readonly defaultContainerType: string;
}


const defaultSettings: ServerSettings = {
    availableNameGenerators: [],
    databaseConfigurationsEnabled: false,
    defaultContainerType: "",
    contentUrl: "",
    structureUpdateEnabled: false
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

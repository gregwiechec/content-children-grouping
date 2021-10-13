export interface GroupConfiguration {
    contentLink: string;
    fromCode: boolean;
    routingEnabled: boolean;
    isVirtualContainer: boolean;
    containerTypeName: string;
    groupLevelConfigurations: string[];
    contentExists?: boolean;
    changedBy?: string;
    changedOn?: string;
}

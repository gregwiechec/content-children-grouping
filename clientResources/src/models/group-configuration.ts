export interface GroupConfiguration {
  contentLink: string;
  fromCode: boolean;
  routingEnabled: boolean;
  isVirtualContainer: boolean;
  containerTypeName: string;
  groupLevelConfigurations: GeneratorConfiguration[];
  contentExists?: boolean;
  changedBy?: string;
  changedOn?: string;
}

export interface GeneratorConfiguration {
  name: string;
  settings?: Record<string, string>;
}

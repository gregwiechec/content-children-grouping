export interface GroupConfiguration {
  contentLink: string;
  fromCode: boolean;
  groupLevelConfigurations: GeneratorConfiguration[];
  contentExists?: boolean;
  changedBy?: string;
  changedOn?: string;
}

export interface GeneratorConfiguration {
  name: string;
  settings?: Record<string, string>;
}

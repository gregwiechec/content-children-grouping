import { DataService } from "../data-service";
import { GroupConfiguration } from "../models/group-configuration";

const items: GroupConfiguration[] = [
  {
    contentLink: "1",
    fromCode: false,
    groupLevelConfigurations: [{ name: "Name" }, { name: "Create Date" }]
  },
  {
    contentLink: "2",
    fromCode: true,
    groupLevelConfigurations: [{ name: "Name" }, { name: "Create Date" }]
  },
  {
    contentLink: "3",
    fromCode: true,
    groupLevelConfigurations: [{ name: "Name" }, { name: "Create Date" }]
  },
  {
    contentLink: "4",
    fromCode: false,
    groupLevelConfigurations: [{ name: "Name" }]
  }
];

let result = {
  items: items,
  availableNameGenerators: ["Name", "Create Date", "Very long name generator"],
  structureUpdateEnabled: false
};

export const fakeService: DataService = {
  load: () => {
    return new Promise((resolve) => {
      setTimeout(() => {
        resolve(result);
      }, 500);
    });
  },
  get: (contentLink: string) => {
    return new Promise((resolve) => {
      setTimeout(() => {
        resolve(result.items.filter((x) => x.contentLink === contentLink)[0]);
      }, 500);
    });
  },
  save: (configuration: GroupConfiguration) => {
    if (configuration.contentLink === "111") {
      return new Promise((resolve, reject) => reject({ message: "Cannot save item" }));
    }
    return new Promise((resolve) => setTimeout(() => resolve(true), 2000));
  },
  delete(configuration: GroupConfiguration): Promise<GroupConfiguration[]> {
    return new Promise((resolve) =>
      setTimeout(() => {
        const index = result.items.indexOf(result.items.filter((x) => x.contentLink === configuration.contentLink)[0]);
        if (index > -1) {
          result.items.splice(index, 1);
        }
        resolve(result.items);
      }, 2000)
    );
  }
};

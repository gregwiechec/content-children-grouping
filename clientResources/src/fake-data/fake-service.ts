import { DataService } from "../data-service";
import { GroupConfiguration } from "../models/group-configuration";

const items: GroupConfiguration[] = [
  {
    contentLink: "1",
    fromCode: false,
    routingEnabled: true,
    containerTypeName: "Alloy.CustomContainer",
    groupLevelConfigurations: [{ name: "Name" }, { name: "Create Date" }],
    isVirtualContainer: true
  },
  {
    contentLink: "2",
    fromCode: true,
    routingEnabled: false,
    containerTypeName: "",
    groupLevelConfigurations: [{ name: "Name" }, { name: "Create Date" }],
    isVirtualContainer: true
  },
  {
    contentLink: "3",
    fromCode: true,
    routingEnabled: false,
    containerTypeName: "",
    groupLevelConfigurations: [{ name: "Name" }, { name: "Create Date" }],
    isVirtualContainer: false
  },
  {
    contentLink: "4",
    fromCode: false,
    routingEnabled: true,
    containerTypeName: "",
    groupLevelConfigurations: [{ name: "Name" }],
    isVirtualContainer: false
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
  clearContainers: (contentLink: string) => {
    return new Promise((resolve) =>
      setTimeout(() => {
        resolve({ data: `Containers cleared successfully (${contentLink})` });
      }, 3000)
    );
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

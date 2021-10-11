import {DataService} from "../data-service";

const result = {
  items: [
    {
      contentLink: "1",
      fromCode: false,
      routingEnabled: true,
      containerTypeName: "Alloy.CustomContainer",
      groupLevelConfigurations: ["Name", "Created Date"],
      isVirtualContainer: true
    },
    {
      contentLink: "2",
      fromCode: true,
      routingEnabled: false,
      isVirtualContainer: true,
      containerTypeName: "",
      groupLevelConfigurations: ["Name", "Created Date"]
    },
    {
      contentLink: "3",
      fromCode: true,
      routingEnabled: false,
      containerTypeName: "",
      groupLevelConfigurations: ["Name", "Created Date"],
      isVirtualContainer: false,
    },
    {
      contentLink: "4",
      fromCode: false,
      routingEnabled: true,
      containerTypeName: "",
      groupLevelConfigurations: ["Name"],
      isVirtualContainer: false
    }
  ],
  availableNameGenerators: ["Name", "Created Date", "Very long name generator"],
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
        resolve(result.items.filter(x => x.contentLink === contentLink)[0]);
      }, 500);
    });
  },
  save: () => {
    return new Promise((resolve) => resolve(false));
  },
  clearContainers: (contentLink: string) => {
    return new Promise((resolve) =>
      setTimeout(() => {
        resolve(`Containers cleared successfully (${contentLink})`);
      }, 3000)
    );
  }
};

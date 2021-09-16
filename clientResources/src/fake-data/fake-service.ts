export const fakeService = {
    load: () => {
        return new Promise((resolve) => {
            setTimeout(() => {
                const result = {
                    items: [
                        {
                            contentLink: "123",
                            routingEnabled: false,
                            containerTypeName: "",
                            groupLevelConfigurations: ["Name", "Created Date"]
                        },
                        {
                            contentLink: "124",
                            routingEnabled: true,
                            containerTypeName: "",
                            groupLevelConfigurations: ["Name"]
                        }
                    ],
                    availableNameGenerators: ["Name", "Created Date", "Very long name generator"],
                    structureUpdateEnabled: false
                };
                resolve(result);
            }, 500);
        });
    },
    save: () => {
        return new Promise((resolve) => resolve(false));
    },
    clearContainers: (contentLink: string) => {
        return new Promise((resolve) =>
            setTimeout(() => {
                resolve("Containers cleared successfully");
            }, 3000)
        );
    }
};

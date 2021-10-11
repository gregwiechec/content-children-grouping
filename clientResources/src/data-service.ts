import axios from "axios";
import { createContext, useContext } from "react";
import { GroupConfiguration } from "./models/Groupconfiguration";

export interface DataService {
  load: () => Promise<any>;
  save: (configurations: any) => Promise<any>;
  get: (contentLink: string) => Promise<GroupConfiguration>;
  clearContainers: (contentLink: string) => Promise<any>;
}

export const dataService: DataService = {
  load: () => {
    return axios
      .get("LoadConfigurations")
      .then((response) => {
        return response.data;
      })
      .catch((error) => {
        console.error(error);
        return {};
      });
  },

  get: (contentLink: string) => {
    return axios
      .get("get/" + contentLink)
      .then((response) => {
        return response.data;
      })
      .catch((error) => {
        console.error(error);
        return {};
      });
  },

  save: (configuration: any) => {
    return axios.post("Save", configuration);
  },

  clearContainers: (contentLink: string) => {
    return axios.post("ClearContainers", { contentLink: contentLink });
  }
};

const DataServiceContext = createContext<DataService>(dataService);

export default DataServiceContext;

export const useDataServiceContext = (): DataService => {
  const dataServiceContext = useContext(DataServiceContext);
  if (!dataServiceContext) {
    throw new Error("dataServiceContext must be used within the ServerSettingsContext.Provider");
  }
  return dataServiceContext;
};

import axios from "axios";
import { createContext, useContext } from "react";
import { GroupConfiguration } from "./models/group-configuration";

export interface DataService {
  load: () => Promise<any>;
  save: (configurations: any) => Promise<any>;
  get: (contentLink: string) => Promise<GroupConfiguration>;
  delete(configuration: GroupConfiguration): Promise<GroupConfiguration[]>;
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
      .get("get?contentLink=" + contentLink)
      .then((response) => {
        return response.data;
      })
      .catch((error) => {
        console.error(error);
        return {};
      });
  },

  save: (configuration: any) => {
    return new Promise((resolve, reject) => {
      axios.post("Save", configuration).then(result => {
        if (result.data.status === 400 && result.data.error) {
          reject(result.data.error);
          return;
        }
        resolve(result);
      }).catch(error => {
        reject(error.message || error.toString());
      });
    });
  },

  delete: (configuration: GroupConfiguration) => {
    return axios
      .delete("Delete?contentLink=" + configuration.contentLink)
      .then((result) => result.data)
      .catch((error) => {
        console.error(error);
        return {};
      });
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

/* TODO: add tests */

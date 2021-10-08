import axios from "axios";

export interface DataService {
  load: () => Promise<any>;
  save: (configurations: any) => Promise<any>;
  get: (contentLink: string) => Promise<any>;
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

  save: (configurations: any[]) => {
    return axios.post("Save", configurations);
  },

  clearContainers: (contentLink: string) => {
    return axios.post("ClearContainers", { contentLink: contentLink });
  }
};

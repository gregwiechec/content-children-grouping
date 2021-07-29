import axios from "axios";

export interface DataService {
  load: () => Promise<any>;
  save: (configurations: any[]) => Promise<any>;
}

export const dataService: DataService = {
  load: () => {
    return axios
      .get("LoadConfigurations") //TODO: path to episerver
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
  }
};

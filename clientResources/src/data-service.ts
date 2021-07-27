import axios from "axios";

export interface DataService {
  load: () => Promise<any>;
  save: (configurations: any[]) => Promise<any>;
}

export const dataService: DataService = {
  load: () => {
    return axios
      .get("/EPiServer/content-children-grouping/ConfigSettings/LoadConfigurations")
      .then((response) => {
        const result = JSON.parse(response.data.substring(4));
        return result;
      })
      .catch((error) => {
        console.error(error);
        return {};
      });
  },

  save: (configurations: any[]) => {
    return axios
      .post("/EPiServer/content-children-grouping/ConfigSettings/Save", configurations)
      .then((response) => {
        console.log(response);
      })
      .catch((error) => {
        console.error(error);
      });
  }
};

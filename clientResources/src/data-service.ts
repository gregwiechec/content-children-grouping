import axios from "axios";

export const dataService = {
  load: () => {
    return axios
      .get("/EPiServer/content-children-grouping/ConfigSettings/LoadConfigurations")
      .then(response => {
        alert("loaded");
        return response;
      })
      .catch((error) => {
        console.error(error);
        return {};
      });
  },

  save: (configurations: any[]) => {
    axios
      .post("/EPiServer/content-children-grouping/ConfigSettings/Save", configurations)
      .then((response) => {
        console.log(response);
      })
      .catch((error) => {
        console.error(error);
      });
  }
};

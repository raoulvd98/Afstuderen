import axios from 'axios'

export default {
  getModels () {
    return axios.get(`/Model`)
      .then(response => {
        return response.data
      })
  },

  trainModel (model) {
    return axios.get(`/Model/${model}`)
      .then(response => {
        return response.data
      })
  }
}

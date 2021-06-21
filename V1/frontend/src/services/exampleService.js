import axios from 'axios'

export default {
  getNumberOfPages (cat) {
    return axios.get(`/Example/count?category=${cat}`)
      .then(response => {
        return response.data
      })
  },

  getCategories () {
    return axios.get(`/Model`)
      .then(response => {
        return response.data
      })
  },

  getExamples (pageNum, cat) {
    return axios.get(`/Example?page=${pageNum}&category=${cat}`)
      .then(response => {
        return response.data
      })
  },

  getSingleExample (id) {
    return axios.get(`/Example/${id}`)
      .then(response => {
        return response.data
      })
  },

  createSingleExample (example) {
    return axios.post(`/Example`, {
      category: example.category,
      example: example.example,
      isPositive: example.isPositive
    })
      .then(response => {
        return response.data.id
      })
  },

  updateSingleExample (example) {
    return axios.put(`/Example`, {
      id: example.id,
      category: example.category,
      example: example.example,
      isPositive: example.isPositive
    })
      .then(() => { return 'Updated' })
  },

  deleteSingleExampele (example) {
    return axios.delete(`/Example`, { data: {
      id: example.id,
      category: example.category,
      example: example.example,
      isPositive: example.isPositive
    }
    })
      .then(() => { return 'deleted' })
  }
}

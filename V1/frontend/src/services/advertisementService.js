import axios from 'axios'

export default {
  getNumberOfPages (brand, company) {
    return axios.get(`/Advertisement/count?brand=${brand}&company=${company}`)
      .then(response => {
        return response.data
      })
  },
  getAdvertisements (pageNum, brand, company) {
    return axios.get(`/Advertisement?page=${pageNum}&brand=${brand}&company=${company}`)
      .then(response => {
        return response.data
      })
  },
  getSingleAdvertisement (id) {
    return axios.get(`/Advertisement/${id}`)
      .then(response => {
        return response.data
      })
  }
}

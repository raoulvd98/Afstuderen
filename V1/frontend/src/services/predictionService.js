import axios from 'axios'

export default {
  getNumberOfPages (model, positive) {
    return axios.get(`/Prediction/count?&model=${model}&positive=${positive}`)
      .then(response => {
        return response.data
      })
  },

  getPredictions (pageNum, model, positive, orderByModel) {
    return axios.get(`/Prediction?page=${pageNum}&model=${model}&positive=${positive}&orderByModel=${orderByModel}`)
      .then(response => {
        return response.data
      })
  },

  getSinglePrediction (id) {
    return axios.get(`/Prediction/${id}`)
      .then(reponse => {
        return reponse.data
      })
  },

  creatNewPrediction (id) {
    return axios.post(`/Prediction/${id}`)
      .then(reponse => {
        return reponse.data
      })
  },

  updateSinglePrediction (prediction) {
    return axios.put(`/Prediction`, {
      PredictionId: prediction.predictionId,
      AdvertisementId: prediction.advertisementId,
      Model: prediction.model,
      GoodSentence: prediction.goodSentence,
      WrongSentence: prediction.wrongSentence,
      Date: prediction.date
    })
      .then(() => { return 'Updated' })
  },

  deleteSinglePrediction (prediction) {
    return axios.delete(`/Prediction`, { data: {
      PredictionId: prediction.predictionId,
      AdvertisementId: prediction.advertisementId,
      Model: prediction.model,
      GoodSentence: prediction.goodSentence,
      WrongSentence: prediction.wrongSentence,
      Date: prediction.date
    }
    })
      .then(() => { return 'deleted' })
  }

}

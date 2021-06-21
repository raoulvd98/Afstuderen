<template>
  <section>
      <h1>Voorspellingen</h1>
      <div class="loading" v-if="loading">
        <b-spinner small variant="secondary" label="Spinning"></b-spinner> loading ...
      </div>
      <div v-if="error" >
        <b-alert variant="danger" show dismissible>
          {{ error }} <br>
          {{ error.response.data}}
        </b-alert>
      </div>

      <div class="d-flex justify-content-between">
        <b-button v-b-toggle.filter variant="outline-secondary" size="sm" class="m-1 mb-2">Toggle filter</b-button>
      </div>

      <b-collapse id="filter" class="mb-2">
        <b-card>
          <b-form-group id="fieldset-horizontal" label-cols-sm="4" label-cols-lg="3" label="Categorie :" label-for="category">
            <b-form-input id="category" list="cat-list" v-model="filterCategory"></b-form-input>
            <datalist id="cat-list">
              <option v-for="cat in categories" :key="cat">{{ cat }}</option>
            </datalist>
          </b-form-group>
          <b-form-group id="fieldset-horizontal" label-cols-sm="4" label-cols-lg="3" label="Advertenties die zijn:" label-for="isPositive">
            <b-form-input id="isPositive" list="isPositive-list" v-model="filterIsPositive"></b-form-input>
            <datalist id="isPositive-list">
              <option v-for="IsPositive in IsPositiveValues" :key="IsPositive">{{ IsPositive }}</option>
            </datalist>
          </b-form-group>
          <b-form-checkbox v-model="orderByModel" name="check-button" switch>
            Sorteer op model
          </b-form-checkbox><br>
          <b-button variant="secondary" v-on:click="filter()">Filter</b-button>
        </b-card>
      </b-collapse>

      <b-list-group>
        <b-list-group-item v-for="prediction in predictions" :key="prediction.predictionId" class="flex-column align-items-start" :to="{ name: 'singleAdvertisement', params: { id: prediction.advertisementId }}">
          <div class="d-flex w-100 justify-content-between align-items-center">
            <h5 class="mb-0">{{ prediction.advertisementId }} - {{ prediction.model }} </h5>
            <small class="d-flex justify-content-end">{{ prediction.date }}</small>
          </div>

          <p class="mb-1">
            Kans goed : {{ (prediction.goodSentence * 100).toFixed(2) }}% <br>
            Kans fout : {{ (prediction.wrongSentence * 100).toFixed(2) }}%
          </p>
        </b-list-group-item>
      </b-list-group>

      <b-list-group v-if="!predictions">
        <b-list-group-item class="flex-column align-items-start">
          <div class="d-flex w-100 justify-content-between">
            <h5 class="mb-1">404</h5>
            <small>-</small>
          </div>

          <p class="mb-1">
            Er zijn geen Voorspellingen gevonden!
          </p>

        </b-list-group-item>
      </b-list-group>

      <div class="mt-3">
        <b-pagination-nav :link-gen="linkGen" :number-of-pages="pages" use-router first-number last-number align="center">
        </b-pagination-nav>
      </div>

  </section>
</template>

<script>
import PredictionApi from '@/services/predictionService'
import ModelApi from '@/services/modelService'
export default {
  name: 'Prediciton',

  data () {
    return {
      loading: true,
      error: null,
      predictions: [],
      categories: [],
      IsPositiveValues: ['Goedgekeurd', 'Afgekeurd'],
      filterCategory: '',
      filterIsPositive: '',
      orderByModel: false,
      pages: 1,
      currentPage: 1
    }
  },

  watch: {
    '$route.query.page': function (page) {
      if (page === undefined) {
        page = 1
      }
      this.currentPage = page
      this.loadData()
    }
  },

  methods: {
    linkGen (pageNum) {
      return pageNum === 1 ? '?' : `?page=${pageNum}`
    },
    filter () {
      this.loadData()
    },
    loadData () {
      ModelApi.getModels()
        .then(categories => {
          this.categories = categories
        })
        .catch(error => {
          this.error = error
        })
        .finally(() => {
          this.loading = false
        })

      var filterToSent = ''
      if (this.filterIsPositive === 'Goedgekeurd') {
        filterToSent = 'true'
      } else if (this.filterIsPositive === 'Afgekeurd') {
        filterToSent = 'false'
      }

      PredictionApi.getNumberOfPages(this.filterCategory, filterToSent)
        .then(pages => {
          if (pages > 1) {
            this.pages = Math.ceil(pages / 10)
          } else if (pages <= 1) {
            this.pages = 1
          }
        })
        .catch(error => {
          this.error = error
        })

      if (this.currentPage > this.pages) {
        this.currentPage = this.pages
      }

      PredictionApi.getPredictions(this.currentPage, this.filterCategory, filterToSent, this.orderByModel)
        .then(predictions => {
          this.predictions = predictions
        })
        .catch(error => {
          this.error = error
        })
        .finally(() => {
          this.loading = false
        })
    }
  },

  mounted () {
    this.currentPage = this.$route.query.page
    if (this.currentPage === undefined) {
      this.currentPage = 1
    }

    this.filterIsPositive = this.$route.query.predictions
    if (this.filterIsPositive !== 'True' && this.filterIsPositive !== 'False') {
      this.filterIsPositive = ''
    } else if (this.filterIsPositive === 'False') {
      this.filterIsPositive = 'Afgekeurd'
    } else if (this.filterIsPositive === 'True') {
      this.filterIsPositive = 'Goedgekeurd'
    }

    this.loadData()
  }
}
</script>

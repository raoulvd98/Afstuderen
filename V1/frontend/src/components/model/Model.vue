<template>
  <section>
      <h1>Modellen</h1>
      <div class="loading" v-if="loading">
        <b-spinner small variant="secondary" label="Spinning"></b-spinner> loading ...
      </div>
      <div v-if="error" >
        <b-alert variant="danger" show dismissible>
          {{ error }} <br>
          {{ error.response.data}}
        </b-alert>
      </div>

      <b-list-group>
        <b-list-group-item v-for="model in models" :key="model" class="flex-column align-items-start">
          <div class="d-flex w-100 justify-content-between align-items-center">
            <h5 class="mb-0">{{ model }}</h5>
            <b-button variant="secondary" size="sm" :to="{ name: 'trainModel', params: { id: model }}">Train {{ model }}-model</b-button>
          </div>
        </b-list-group-item>
      </b-list-group>

  </section>
</template>

<script>
import ModelApi from '@/services/modelService'
export default {
  name: 'Model',

  data () {
    return {
      loading: true,
      error: null,
      models: []
    }
  },

  mounted () {
    ModelApi.getModels()
      .then(models => {
        this.models = models
      })
      .catch(error => {
        this.error = error
      })
      .finally(() => {
        this.loading = false
      })
  }
}
</script>

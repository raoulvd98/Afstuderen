<template>
  <section>
      <h1>Modellen</h1>
      <div class="loading d-flex justify-content-center" v-if="loading">
        <b-spinner small variant="secondary" label="Spinning"></b-spinner> Aan het trainen, een moment geduld aub ...
      </div>
      <div v-else>
        <h4>Trainingsresultaten van {{ model }}</h4>
      </div>
      <div v-if="error" >
        <b-alert variant="danger" show dismissible>
          {{ error }} <br>
          {{ error.response.data}}
        </b-alert>
      </div>

      <div>
        <b-table striped hover :items="trainResults" :fields="fields"></b-table>
      </div>

  </section>
</template>

<script>
import ModelApi from '@/services/modelService'
export default {
  name: 'trainModel',

  data () {
    return {
      loading: true,
      error: null,
      fields: ['Loss', 'Precision', 'Recall', 'F-score'],
      trainResults: [],
      model: null
    }
  },

  mounted () {
    ModelApi.trainModel(this.$route.params.id)
      .then(result => {
        this.trainResults = result
      })
      .catch(error => {
        this.error = error
      })
      .finally(() => {
        this.model = this.$route.params.id
        this.loading = false
      })
  }
}
</script>

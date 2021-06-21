<template>
  <section>
      <h1>Maak een nieuw voorbeeld aan</h1>
      <div class="loading" v-if="loading">
        <b-spinner small variant="secondary" label="Spinning"></b-spinner> loading ...
      </div>
      <div v-if="error" >
        <b-alert variant="danger" show dismissible>
          {{ error }} <br>
          {{ error.response.data}}
        </b-alert>
      </div>

      <b-modal id="unsuccesful-update" ref="unsuccesful-create" hide-footer>
        <p class="my-4">Er is iets fout gegaan! Controleer of alle velden goed zijn ingevuld!</p>
        <b-button variant="success" v-on:click="hideModal()">Close</b-button>
      </b-modal>

      <b-form-group id="fieldset-horizontal" label-cols-sm="3" label-cols-lg="2" label="Voorbeeld" label-for="example">
        <b-form-input id="example" v-model="example.example" :state="exampleState" aria-describedby="input-live-feedback-exmaple"></b-form-input>
        <b-form-invalid-feedback id="input-live-feedback-example">
            Vul minstens vijf karakters in!
        </b-form-invalid-feedback>
      </b-form-group>

      <b-form-group id="fieldset-horizontal" label-cols-sm="3" label-cols-lg="2" label="Positief voorbeeld?" label-for="isPositive">
        <b-form-select v-model="example.isPositive" :options="isPositiveOptions" :state="isPositiveState" aria-describedby="input-live-feedback-isPositive"></b-form-select>
        <b-form-invalid-feedback id="input-live-feedback-isPositive">
            Selecteer een optie
        </b-form-invalid-feedback>
      </b-form-group>

      <b-form-group id="fieldset-horizontal" label-cols-sm="3" label-cols-lg="2" label="Categorie :" label-for="category">
        <b-form-input id="category" list="my-list-id" v-model="example.category" :state="categoryState" aria-describedby="input-live-feedback-category"></b-form-input>
        <b-form-invalid-feedback id="input-live-feedback-category">
            Deze categorie bestaat niet!
        </b-form-invalid-feedback>
        <datalist id="my-list-id">
          <option v-for="cat in categories" :key="cat">{{ cat }}</option>
        </datalist>
      </b-form-group>

      <div class="d-flex justify-content-between">
        <b-button variant="info" :to="{ name: 'Example'}">Ga terug</b-button>
        <b-button variant="success" v-on:click="createExample()">Maak</b-button>
      </div>

  </section>
</template>

<script>
import ExampleAPI from '@/services/exampleService'
export default {
  name: 'addExample',

  data () {
    return {
      loading: true,
      error: null,
      example: [],
      categories: [],
      isPositiveOptions: [
        {value: null, text: 'Selecteer een optie'},
        {value: true, text: 'True'},
        {value: false, text: 'False'}
      ]
    }
  },

  computed: {
    exampleState () {
      if (this.example.example === undefined) {
        return false
      }
      return this.example.example.length > 5
    },
    isPositiveState () {
      if (this.example.isPositive === true || this.example.isPositive === false) {
        return true
      }
      return false
    },
    categoryState () {
      if (this.categories.includes(this.example.category)) {
        return true
      }
      return false
    }
  },

  methods: {
    createExample () {
      if (this.exampleState && this.isPositiveState && this.categoryState) {
        ExampleAPI.createSingleExample(this.example)
          .then(exampleId => {
            this.$router.push({ name: 'singleExample', params: { id: exampleId } })
          })
          .catch(error => {
            this.status = null
            this.error = error
          })
      }
      this.$refs['unsuccesful-create'].show()
    },
    hideModal () {
      this.$refs['unsuccesful-create'].hide()
    }
  },

  mounted () {
    this.example = {
      example: '',
      isPositive: '',
      category: ''
    }
    ExampleAPI.getCategories()
      .then(categories => {
        this.categories = categories
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

<template>
  <section>
      <h1>Los voorbeeld</h1>
      <div class="loading" v-if="loading">
        <b-spinner small variant="secondary" label="Spinning"></b-spinner> loading ...
      </div>
      <div v-if="error" >
        <b-alert variant="danger" show dismissible>
          {{ error }} <br>
          {{ error.response.data}}
        </b-alert>
      </div>

      <b-modal id="succesful-update" ref="succesful-update" hide-footer>
        <p class="my-4">Het voorbeeld is succesvol geüpdatet</p>
        <b-button variant="success" v-on:click="hideModal()">Close</b-button>
      </b-modal>
      <b-modal id="unsuccesful-update" ref="unsuccesful-update" hide-footer>
        <p class="my-4">Er is iets fout gegaan! Controleer of alle velden goed zijn ingevuld!</p>
        <b-button variant="success" v-on:click="hideModal()">Close</b-button>
      </b-modal>
      <b-modal id="delete" ref="delete" hide-footer>
        <p class="my-4">Weet u zeker dat u dit voorbeeld wilt verwijderen?</p>
        <b-button variant="success" v-on:click="hideModal()">Nee, toch niet!</b-button>
        <b-button variant="danger" v-on:click="deleteExample()">Ja, Verwijderen!</b-button>
      </b-modal>

      <div v-if="example">
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
      </div>

      <div class="d-flex justify-content-between">
        <b-button variant="info" :to="{ name: 'Example'}">Ga terug</b-button>
        <div v-if="example">
          <b-button variant="warning" v-on:click="updateExample()">Updaten</b-button>
          <b-button variant="danger" v-on:click="deletePreCheck()">Verwijderen</b-button>
        </div>
      </div>

  </section>
</template>

<script>
import ExampleAPI from '@/services/exampleService'
export default {
  name: 'singleExample',

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
    updateExample () {
      if (this.exampleState && this.isPositiveState && this.categoryState) {
        ExampleAPI.updateSingleExample(this.example)
          .then(() => {
            this.$refs['succesful-update'].show()
          })
          .catch(error => {
            this.status = null
            this.error = error
          })
      }
      this.$refs['unsuccesful-update'].show()
    },
    deletePreCheck () {
      this.$refs['delete'].show()
    },
    deleteExample () {
      ExampleAPI.deleteSingleExampele(this.example)
        .then(() => {
          this.$router.push({ name: 'Example' })
        })
        .catch(error => {
          this.status = null
          this.error = error
        })
    },
    hideModal () {
      this.$refs['succesful-update'].hide()
      this.$refs['unsuccesful-update'].hide()
      this.$refs['delete'].hide()
    }
  },

  mounted () {
    ExampleAPI.getSingleExample(this.$route.params.id)
      .then(example => {
        this.example = example
      })
      .catch(error => {
        this.example = null
        this.error = error
      })
      .finally(() => {
        this.loading = false
      })
    ExampleAPI.getCategories()
      .then(categories => {
        this.categories = categories
      })
      .catch(error => {
        this.error = error
      })
  }
}
</script>

<template>
  <section>
      <h1>Voorbeelden</h1>
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
        <b-button :to="{ name: 'addExample'}" variant="outline-success" size="sm" class="m-1 mb-2">Nieuw voorbeeld</b-button>
      </div>

      <b-collapse id="filter" class="mb-2">
        <b-card>
          <b-form-group id="fieldset-horizontal" label-cols-sm="3" label-cols-lg="2" label="Categorie :" label-for="category">
            <b-form-input id="category" list="my-list-id" v-model="filterCategory"></b-form-input>
            <datalist id="my-list-id">
              <option v-for="cat in categories" :key="cat">{{ cat }}</option>
            </datalist>
          </b-form-group>
          <b-button variant="secondary" v-on:click="filter()">Filter</b-button>
        </b-card>
      </b-collapse>

      <b-list-group>
        <b-list-group-item v-for="example in examples" :key="example.id" class="flex-column align-items-start" :to="{ name: 'singleExample', params: { id: example.id }}" action>
          <div class="d-flex w-100 justify-content-between">
            <h5 class="mb-1">{{ example.category }}</h5>
            <small>{{ example.isPositive }}</small>
          </div>

          <p class="mb-1">
            {{ example.example }}
          </p>

        </b-list-group-item>
      </b-list-group>

      <b-list-group v-if="!examples">
        <b-list-group-item class="flex-column align-items-start">
          <div class="d-flex w-100 justify-content-between">
            <h5 class="mb-1">404</h5>
            <small>-</small>
          </div>

          <p class="mb-1">
            Er zijn geen voorbeelden gevonden in deze category
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
import ExampleAPI from '@/services/exampleService'
export default {
  name: 'Example',

  data () {
    return {
      loading: true,
      error: null,
      examples: [],
      categories: [],
      filterCategory: '',
      currentPage: 1,
      pages: 1
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
      ExampleAPI.getNumberOfPages(this.filterCategory)
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

      ExampleAPI.getCategories()
        .then(categories => {
          this.categories = categories
        })
        .catch(error => {
          this.error = error
        })

      ExampleAPI.getExamples(this.currentPage, this.filterCategory)
        .then(examples => {
          this.examples = examples
        })
        .catch(error => {
          this.error = error
        })
        .finally(() => {
          this.loading = false
        })
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

  mounted () {
    this.currentPage = this.$route.query.page
    if (this.currentPage === undefined) {
      this.currentPage = 1
    }

    this.filterCategory = this.$route.query.category
    if (this.filterCategory === undefined) {
      this.filterCategory = ''
    }

    this.loadData()
  }
}
</script>

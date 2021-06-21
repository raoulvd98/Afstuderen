<template>
  <section>
      <h1>Advertenties</h1>
      <div class="loading" v-if="loading">
        <b-spinner small variant="secondary" label="Spinning"></b-spinner> loading ...
      </div>
      <div v-if="error" >
        <b-alert variant="danger" show dismissible>
          {{ error }} <br>
          {{ error.response.data}}
        </b-alert>
      </div>

      <b-button v-b-toggle.filter variant="outline-secondary" size="sm" class="m-1 mb-2">Toggle filter</b-button>
      <b-collapse id="filter" class="mb-2">
        <b-card class="filter">
          <b-form-group id="fieldset-horizontal" label-cols-sm="3" label-cols-lg="2" label="Merk :" label-for="brand" style="width: 100%">
            <b-form-input id="brand" v-model="filterBrand"></b-form-input>
          </b-form-group>
          <b-form-group id="fieldset-horizontal" label-cols-sm="3" label-cols-lg="2" label="Eigenaar :" label-for="owner" style="width: 100%">
            <b-form-input id="owner"  v-model="filterCompany"></b-form-input>
          </b-form-group>
          <b-button variant="dark" v-on:click="filter()" class="filter">Filter</b-button>
        </b-card>
      </b-collapse>

      <b-card-group deck v-for="advertisement in advertisements" :key="advertisement.id">
        <b-card no-body class="overflow-hidden">
          <b-row no-gutters>
            <b-col md="4" v-if="advertisement.Advertisement.Documentatie.Afbeeldingen[0]" >
              <b-card-img v-bind:src="advertisement.Advertisement.Documentatie.Afbeeldingen[0].Uri" alt="Kan de opgevraagde afbeelding niet vinden" class="rounded-0"></b-card-img>
            </b-col>
            <b-col md="4" v-else>

            </b-col>
            <b-col md="8">
              <b-card-body>
                <div style="width: 100%;">
                  <b-card-title>
                    {{ advertisement.Advertisement.Auto.Algemeen.Titel }}
                  </b-card-title>
                  <b-card-text>
                    &euro; {{ advertisement.Advertisement.Auto.Prijs.AllInPrijs }}
                      <span v-if="advertisement.Advertisement.Auto.Prijs.ExclusiefBtw === true"> Exclusief BTW</span>
                      <span v-else > Inclusief BTW </span>
                      <br>
                      {{ advertisement.Advertisement.Auto.Algemeen.OpmerkingenConsument}}
                  </b-card-text>
                </div>
                <b-button :to="{ name: 'singleAdvertisement', params: { id: advertisement.id }}" variant="outline-secondary" size="sm" class="advertisement">Bekijk advertentie</b-button>
              </b-card-body>
            </b-col>
          </b-row>
        </b-card>
      </b-card-group>
      <div class="loading d-flex justify-content-center" v-if="loading && Object.keys(advertisements).length !== 0">
        <b-spinner small variant="secondary" label="Spinning"></b-spinner> loading ...
      </div>
      <div class="mt-3">
        <b-pagination-nav :link-gen="linkGen" :number-of-pages="pages" use-router first-number last-number align="center">
        </b-pagination-nav>
      </div>

  </section>
</template>

<script>
import AdvertisementApi from '@/services/advertisementService'
export default {
  name: 'Advertisement',

  data () {
    return {
      loading: true,
      error: null,
      advertisements: [],
      filterBrand: '',
      filterCompany: '',
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
      AdvertisementApi.getNumberOfPages(this.filterBrand, this.filterCompany)
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

      AdvertisementApi.getAdvertisements(this.currentPage, this.filterBrand, this.filterCompany)
        .then(advertisements => {
          this.advertisements = advertisements
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
      this.loading = true
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

    this.filterBrand = this.$route.query.brand
    if (this.filterBrand === undefined) {
      this.filterBrand = ''
    }

    this.filterCompany = this.$route.query.company
    if (this.filterCompany === undefined) {
      this.filterCompany = ''
    }

    this.loadData()
  }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
.card {
  margin: 15px;
}

h1, h2 {
  font-weight: normal;
}
ul {
  list-style-type: none;
  padding: 0;
}
li {
  display: inline-block;
  margin: 0 10px;
}

.card.filter {
  margin-left: 0;
  margin-right: 0;
}

.card-body {
  display: flex;
  flex-wrap: wrap;
  align-content: space-between;
  height: 100%;
}

.btn {
  display: flex;
  margin-left: auto;
}

.btn.filter {
  margin-left: 6px;
}

.pagination .page-item.active .page-link {
  background-color: #212121 !important;
  border-color: #212121 !important;
}
</style>

<template>
  <section>
      <h1>Home</h1>
      <div class="loading" v-if="loading">
        <b-spinner small variant="secondary" label="Spinning"></b-spinner> loading ...
      </div>
      <div v-if="error" >
        <b-alert variant="danger" show dismissible>
          {{ error }} <br>
          {{ error.response.data}}
        </b-alert>
      </div>

      <b-card-group deck>
        <b-card  title="Advertenties" img-src="https://picsum.photos/600/300/?image=1" img-alt="Image" img-top tag="advertisement" style="max-width: 20rem;" class="mb-2">
            <b-button :to="{ name: 'Advertisement' }" variant="outline-secondary" >Bekijk alle advertenties</b-button>
        </b-card>

        <b-card  title="Voorspellingen" img-src="https://picsum.photos/600/300/?image=2" img-alt="Image" img-top tag="advertisement" style="max-width: 20rem;" class="mb-2">
            <b-button :to="{ name: 'Prediction' }" variant="outline-secondary" >Bekijk alle voorspellingen</b-button>
        </b-card>

        <b-card  title="Modellen" img-src="https://picsum.photos/600/300/?image=15" img-alt="Image" img-top tag="advertisement" style="max-width: 20rem;" class="mb-2">
            <b-button :to="{ name: 'Model' }" variant="outline-secondary" >Bekijk alle modellen</b-button>
        </b-card>

        <b-card  title="Voorbeelden" img-src="https://picsum.photos/600/300/?image=7" img-alt="Image" img-top tag="advertisement" style="max-width: 20rem;" class="mb-2">
            <b-button :to="{ name: 'Example' }" variant="outline-secondary" >Bekijk alle voorbeelden</b-button>
        </b-card>
      </b-card-group>

      <h2 class="mt-4">Sneltoetsen</h2>
      <b-list-group>
        <b-list-group-item :to="{ name: 'Prediction', query: { predictions: 'False' }}" class="d-flex justify-content-between align-items-center" >
            Laat alle afgekeurde voorspellingen zien!
        </b-list-group-item>
      </b-list-group>

  </section>
</template>

<script>
export default {
  name: 'Home',

  data () {
    return {
      loading: true,
      error: null
    }
  },

  mounted () {
    this.loading = false
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

.card {
  max-width: 20rem;
  min-width: 208px;
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

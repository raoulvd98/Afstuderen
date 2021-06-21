<template>
  <section>
      <h1 v-if="advertisement.Advertisement.Auto">Advertentie - {{ advertisement.Advertisement.Auto.Algemeen.Titel }}</h1>
      <span>{{ advertisement.CreateDateTime }}</span>

      <div class="d-flex">
        <b-col cols="8" class="advertisement-information">
          <div class="loading" v-if="loading_add">
            <b-spinner small variant="secondary" label="Spinning"></b-spinner> loading ...
          </div>
          <div v-if="error_add" >
            <b-alert variant="danger" show dismissible>
              {{ error_add }}
            </b-alert>
          </div>
          <div v-if="error_new" >
                <b-alert variant="danger" show dismissible>
                  {{ error_new }}
                </b-alert>
              </div>
          <b-card-group deck v-if="advertisement.Advertisement.Auto">
            <b-card header="Algemeen"  border-variant="secondary" header-border-variant="secondary">
                <b-card-text>
                    {{ advertisement.Advertisement.Auto.Algemeen.Merk }} {{ advertisement.Advertisement.Auto.Algemeen.Model }} <br>
                    {{ advertisement.Advertisement.Auto.Algemeen.NieuwOccasion }}
                </b-card-text>
            </b-card>
            <b-card header="Prijs" border-variant="secondary" header-border-variant="secondary">
                <b-card-text>
                        &euro; {{ advertisement.Advertisement.Auto.Prijs.AllInPrijs }}
                          <span v-if="advertisement.Advertisement.Auto.Prijs.ExclusiefBtw === true"> Exclusief BTW</span>
                          <span v-else > Inclusief BTW </span>
                </b-card-text>
            </b-card>
          </b-card-group>

          <b-card-group deck>
            <b-card header="Eigenaar"  border-variant="secondary" header-border-variant="secondary">
                <b-card-text>
                    <a :href="'mailto:' + advertisement.Advertisement.CommercieelEigenaar.EmailAlgemeen"> {{ advertisement.Advertisement.CommercieelEigenaar.Naam }} </a><br>
                </b-card-text>
            </b-card>

            <b-card v-if="advertisement.Advertisement.Auto" header="Garanties"  border-variant="secondary" header-border-variant="secondary">
                100% onderhouden? {{ advertisement.Advertisement.Auto.Algemeen.HeeftHonderdProcentOnderhoudenLabel }} <br>
                Bovagchecklist? {{ advertisement.Advertisement.Auto.Algemeen.HeeftBovagChecklist }} <br>
                Bovagafleverbeurt? {{ advertisement.Advertisement.Auto.Algemeen.HeeftBovagAfleverbeurt }} <br>
                Fabrieksgarantie? {{ advertisement.Advertisement.Auto.Garantie.HeeftFabrieksgarantie }} <br>
                Merkgarantie? {{ advertisement.Advertisement.Auto.Garantie.HeeftMerkgarantie }} <br>
                Bovaggarantie? {{ advertisement.Advertisement.Auto.Garantie.Bovaggarantie }}
            </b-card>
          </b-card-group>

          <b-card-group deck v-if="advertisement.Advertisement.Auto.Algemeen.OpmerkingenConsument">
            <b-card header="Opmerkingen Consument"  border-variant="secondary" header-border-variant="secondary">
                {{ advertisement.Advertisement.Auto.Algemeen.OpmerkingenConsument}}
            </b-card>
          </b-card-group>

          <b-card-group deck v-if="advertisement.Advertisement.Auto.Algemeen.OpmerkingenHandel">
            <b-card header="Opmerkingen Handel"  border-variant="secondary" header-border-variant="secondary">
                {{ advertisement.Advertisement.Auto.Algemeen.OpmerkingenHandel}}
            </b-card>
          </b-card-group>

          <b-list-group v-if="Object.keys(advertisement.Advertisement.Auto.Afleverpakketten).length !== 0">
            <b-list-group-item class="header border-secondary">
              Afleverpakketten
            </b-list-group-item>
              <b-list-group-item v-for="pack in advertisement.Advertisement.Auto.Afleverpakketten" :key="pack.Naam" class="border-secondary">
                  <div class="d-flex w-100 justify-content-between">
                      <h5 class="mb-1">{{ pack.Naam }}</h5>
                      <h5 v-if="pack.IsInbegrepen === true"> (inbegrepen)</h5>
                      <h5 v-else-if="pack.IsInbegrepen === false || pack.IsInbegrepen === null"> &euro; {{ pack.Prijs }} </h5>
                  </div>

                  <p class="mb-1">
                      {{ pack.Omschrijving }}
                  </p>
              </b-list-group-item>
          </b-list-group>
        </b-col>

        <b-col class="advertisement-prediction">
          <div class="loading" v-if="loading_pred">
            <b-spinner small variant="secondary" label="Spinning"></b-spinner> loading ...
          </div>
          <b-list-group v-if="error_pred" class="predictions">
            <b-list-group-item class="header border-secondary">
              Controleren
            </b-list-group-item>
            <b-list-group-item variant="warning" class="border-secondary" >
                <p class="mb-1">
                  Er zijn geen voorspellingen gevonden. Wilt u de advertentie nu controleren? <br>
                  <b-button variant="outline-dark" v-on:click="predict()" >Controleer</b-button>
                </p>
            </b-list-group-item>
          </b-list-group>

          <b-list-group v-if="Object.keys(predictions).length !== 0" class="predictions">
            <b-list-group-item class="header border-secondary">
              Voorspellingen advertentie
              <b-button class="mt-2" variant="outline-secondary" size="sm" block v-on:click="newPrediction()">Opnieuw</b-button>
            </b-list-group-item>
              <b-list-group-item v-for="prediction in  predictions" :key="prediction.predictionId" class="border-secondary" v-bind:class="{ 'list-group-item-danger': (prediction.wrongSentence * 100) > 25, 'list-group-item-dark': (prediction.wrongSentence < 0 || prediction.goodSentence < 0) }">
                  <div class="d-flex w-100 justify-content-between">
                      <h5 class="mb-1">{{ prediction.model }}</h5>
                  </div>
                  <p class="mb-1">
                      Kans goed : {{ (prediction.goodSentence * 100).toFixed(2) }}% <br>
                      Kans fout : {{ (prediction.wrongSentence * 100).toFixed(2) }}%
                  </p>
                  <p class="mb-1">
                     <b-button class="mt-2" variant="outline-dark" block v-on:click="showModal(prediction)">Corrigeer</b-button>
                  </p>
              </b-list-group-item>
          </b-list-group>

          <b-modal ref="UpdatePredictionsModal" hide-footer title="Update de voorspelling">
            <div class="d-block text-center">
              <h3>{{ predictionToCorrect.model }}</h3>
               <div v-if="error_update" >
                <b-alert variant="danger" show dismissible>
                  {{ error_update }}
                </b-alert>
              </div>
            </div>
            <b-form-group id="fieldset-horizontal" label-cols-sm="5" label-cols-lg="3" label="kans goed" label-for="chanceGood" style="width: 100%">
              <b-form-input id="chanceGood" v-model="newGoodSentence" :state="percentageStateGood"></b-form-input>
            </b-form-group>
            <b-form-group id="fieldset-horizontal" label-cols-sm="5" label-cols-lg="3" label="Kans fout" label-for="chanceWrong" style="width: 100%">
              <b-form-input id="chanceWrong" v-model="newWrongSentence" :state="percentageStateWrong"></b-form-input>
            </b-form-group>
            {{ predictionToCorrect.newWrongSentence }}
            <div class="d-flex justify-content-between m-2">
              <b-button variant="outline-dark" v-on:click="hideModal()">Sluiten</b-button>
              <b-button variant="success" v-on:click="updaten()">Updaten</b-button>
            </div>
          </b-modal>

        </b-col>

      </div>

  </section>
</template>

<script>
import AdvertisementApi from '@/services/advertisementService'
import PredictionApi from '@/services/predictionService'
export default {
  name: 'Advertisement',

  data () {
    return {
      loading_add: true,
      loading_pred: true,
      error_add: null,
      error_pred: null,
      error_update: null,
      error_new: null,
      advertisement: [],
      predictions: [],
      predictionToCorrect: [],
      newWrongSentence: 0,
      newGoodSentence: 0
    }
  },

  methods: {
    showModal (prediction) {
      this.error_update = null
      this.predictionToCorrect = prediction
      this.newWrongSentence = prediction.wrongSentence * 100
      this.newGoodSentence = prediction.goodSentence * 100
      this.$refs['UpdatePredictionsModal'].show()
    },

    hideModal () {
      this.$refs['UpdatePredictionsModal'].hide()
    },

    updaten () {
      if (this.percentageStateGood && this.percentageStateWrong) {
        this.predictionToCorrect.wrongSentence = this.newWrongSentence / 100
        this.predictionToCorrect.goodSentence = this.newGoodSentence / 100
        PredictionApi.updateSinglePrediction(this.predictionToCorrect)
          .then(() => {
            this.loadData()
            this.hideModal()
          })
          .catch(error => {
            this.error_update = error
          })
      } else {
        this.error_update = 'Er is niet geupdate. Controleer de ingevulde waardes'
      }
    },

    deletePrediction (prediction) {
      console.log(prediction)
      PredictionApi.deleteSinglePrediction(prediction)
        .then(() => {

        })
        .catch(error => {
          this.error_new = error
        })
    },

    newPrediction () {
      this.predictions.forEach(prediction => {
        this.deletePrediction(prediction)
      })
      this.predictions = []
      this.loading_pred = true

      this.predict()
    },

    predict () {
      PredictionApi.creatNewPrediction(this.$route.params.id)
        .then(predictions => {
          this.loading_pred = true
          this.error_pred = null
        })
        .catch(error => {
          this.error_pred = error
        })
        .finally(() => {
          this.loading_pred = false
          this.loadData()
        })
    },

    loadData () {
      AdvertisementApi.getSingleAdvertisement(this.$route.params.id)
        .then(advertisement => {
          this.advertisement = advertisement
        })
        .catch(error => {
          this.error_add = error
        })
        .finally(() => {
          this.loading_add = false
        })

      PredictionApi.getSinglePrediction(this.$route.params.id)
        .then(predictions => {
          this.predictions = predictions
        })
        .catch(error => {
          this.error_pred = error
        })
        .finally(() => {
          this.loading_pred = false
        })
    }
  },

  computed: {
    percentageStateGood () {
      if (this.newGoodSentence >= 0 && this.newGoodSentence <= 100 && this.newGoodSentence !== '') {
        return true
      }
      return false
    },
    percentageStateWrong () {
      if (this.newWrongSentence >= 0 && this.newWrongSentence <= 100 && this.newWrongSentence !== '') {
        return true
      }
      return false
    }
  },

  mounted () {
    this.loadData()
  }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
.card {
  margin: 8px;
  display: flex;
  flex-grow: 1;
}

.card-body {
  display: flex;
  flex-wrap: wrap;
  align-content: space-between;
  height: 100%;
}

.card-deck {
  display: flex;
  flex-grow: 1;
  margin: 0px -8px;
}

.list-group-item.header {
  background-color: rgba(0, 0, 0, 0.03);
}

.list-group {
  margin-top: 8px;
}

.predictions {
  margin: 0 8px 8px 8px;
  width: 100%;
}

.advertisement-information {
  display: flex;
  flex-wrap: wrap;
  flex-grow: 1;
}

.advertisement-prediction{
  display: flex;
  flex-wrap: wrap;
  flex-grow: 1;
  margin: 8px;
}

</style>

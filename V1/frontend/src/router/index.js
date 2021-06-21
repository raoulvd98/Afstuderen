import Vue from 'vue'
import Router from 'vue-router'
import Home from '@/components/Home'
import Advertisement from '@/components/advertisement/Advertisement'
import singleAdvertisement from '@/components/advertisement/singleAdvertisement'
import Example from '@/components/example/Example'
import singleExample from '@/components/example/singleExample'
import addExample from '@/components/example/addExample/'
import Model from '@/components/model/Model'
import trainModel from '@/components/model/trainModel'
import Prediction from '@/components/prediction/Prediction'
import Navbar from '@/components/NavBar'

Vue.component('Navbar', Navbar)
Vue.use(Router)

export default new Router({
  routes: [
    {
      path: '/',
      name: 'Home',
      component: Home
    },
    {
      path: '/advertenties',
      name: 'Advertisement',
      component: Advertisement
    },
    {
      path: '/advertenties/:id',
      name: 'singleAdvertisement',
      component: singleAdvertisement
    },
    {
      path: '/voorbeelden',
      name: 'Example',
      component: Example
    },
    {
      path: '/voorbeelden/add',
      name: 'addExample',
      component: addExample
    },
    {
      path: '/voorbeelden/:id',
      name: 'singleExample',
      component: singleExample
    },
    {
      path: '/model',
      name: 'Model',
      component: Model
    },
    {
      path: '/model/:id',
      name: 'trainModel',
      component: trainModel
    },
    {
      path: '/voorspelling',
      name: 'Prediction',
      component: Prediction
    }
  ]
})

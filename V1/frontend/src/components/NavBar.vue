<template>
  <div>
    <b-navbar type="light" variant="secondary" >
      <div class="container">
        <b-navbar-brand href="/">Advertentie platform</b-navbar-brand>
        <b-navbar-toggle target="nav-collapse"></b-navbar-toggle>

        <b-collapse id="nav-collapse" is-nav>
          <!-- Right aligned nav items -->
          <b-navbar-nav class="ml-auto">
            <router-link :to="{ name: 'Advertisement'}" class="nav-link">Advertenties</router-link>
            <router-link :to="{ name: 'Prediction'}" class="nav-link">Voorspellingen</router-link>
            <router-link :to="{ name: 'Model'}" class="nav-link">Modellen</router-link>
            <router-link :to="{ name: 'Example'}" class="nav-link">Voorbeelden</router-link>

          </b-navbar-nav>
        </b-collapse>
      </div>
    </b-navbar>
  </div>
</template>

<script>
export default {
  name: 'Navbar'
}
</script>

<style scoped>
  .navbar.navbar-light.bg-secondary .nav-link, .navbar.navbar-light.bg-secondary .navbar-brand {
    color: #fff!important;
    transition: .3s;
  }

  .navbar.navbar-light.bg-secondary .nav-link:hover, .navbar.navbar-light.bg-secondary .router-link-active, .navbar.navbar-light.bg-secondary .navbar-brand:hover {
    box-shadow: inset 0px -3px 0px 0px #fff;
  }
</style>

<template>
<div id="app">
  <Navbar :name="$route.name"/>
  <div class="container" style="margin-top:20px">
    <router-view />
  </div>
</div>
</template>

<script>
export default {
  name: 'App'
}
</script>

<style>
#app {
  font-family: 'Avenir', Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  /* color: #2c3e50; */
}
</style>

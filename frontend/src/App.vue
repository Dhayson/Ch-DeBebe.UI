<script setup lang="ts">
import { computed } from 'vue';
import { useRoute } from 'vue-router';
import AppTopbar from './components/AppTopbar.vue';

const route = useRoute();
const showMenu = computed(() => {
    return !['/login', '/cadastro'].includes(route.path);
});
</script>

<!-- <template>
  <header>
    <img alt="Vue logo" class="logo" src="./assets/logo.svg" width="125" height="125" />

    <div class="wrapper">
      <HelloWorld msg="You did it!" />
    </div>
  </header>

  <main>
    <TheWelcome />
  </main>
  <Button></Button>
</template> -->

<template>
    <div class="layout-wrapper">
        <Toast />
        <ConfirmDialog />
        <AppTopbar v-if="showMenu" />
        <main :class="{ 'p-4': showMenu }">
            <router-view />
        </main>
    </div>
</template>

<style scoped>
body {
    margin: 0;
    padding: 0;
    height: 100%;
    background-color: var(--surface-ground);
    font-family: var(--font-family);
    overflow-x: hidden;
}

.content-area {
    margin-top: 4rem; /* Altura aproximada do Menubar */
    padding: 2rem;
}

.layout-content {
    /* Ajuste isso se você usar header fixo (sticky) */
    margin-top: 0.5rem; 
}

header {
  line-height: 1.5;
}

.logo {
  display: block;
  margin: 0 auto 2rem;
}

@media (min-width: 1024px) {
  header {
    display: flex;
    place-items: center;
    padding-right: calc(var(--section-gap) / 2);
  }

  .logo {
    margin: 0 2rem 0 0;
  }

  header .wrapper {
    display: flex;
    place-items: flex-start;
    flex-wrap: wrap;
  }
}
</style>

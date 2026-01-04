<script setup lang="ts">
import { useUIStore } from './stores/ui'
import TopBar from './components/TopBar.vue'
import SideMenu from './components/SideMenu.vue'
import SystemsGrid from './components/SystemsGrid.vue'
import GameGrid from './components/GameGrid.vue'
import GameDetails from './components/GameDetails.vue'
import PlayingView from './components/PlayingView.vue'
import Toast from './components/Toast.vue'

const uiStore = useUIStore()
</script>

<template>
  <div class="app">
    <TopBar v-if="uiStore.currentView !== 'playing'" />
    <SideMenu />
    <main class="main-content">
      <SystemsGrid v-if="uiStore.currentView === 'systems'" />
      <GameGrid v-else-if="uiStore.currentView === 'games'" />
      <GameDetails v-else-if="uiStore.currentView === 'details'" />
      <PlayingView v-else-if="uiStore.currentView === 'playing'" />
    </main>
    <Toast />
  </div>
</template>

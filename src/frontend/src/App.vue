<script setup lang="ts">
import { onMounted } from 'vue'
import { useUIStore } from './stores/ui'
import { useCoresStore } from './stores/cores'
import { useGamesStore } from './stores/games'
import { LibretroApplication } from './generated/libretroApplication'
import { useToast } from './composables/useToast'
import TopBar from './components/TopBar.vue'
import SideMenu from './components/SideMenu.vue'
import SystemsGrid from './components/SystemsGrid.vue'
import GameGrid from './components/GameGrid.vue'
import GameDetails from './components/GameDetails.vue'
import PlayingView from './components/PlayingView.vue'
import Toast from './components/Toast.vue'

const uiStore = useUIStore()
const coresStore = useCoresStore()
const gamesStore = useGamesStore()
const { addToast } = useToast()

onMounted(async () => {
  try {
    await coresStore.initialize()
    await gamesStore.loadLibrary()
  } catch (e) {
    addToast('Error loading cores: ' + e, 'error')
  }

  LibretroApplication.OnDownloadProgress((data) => {
    const core = coresStore.cores.find((c) => c.name === data.Name)
    if (core) {
      core.setProgress(data.Progress)
    }
  })
})
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

<style>
@import './styles/main.css';
</style>

<style scoped>
.app {
  height: 100vh;
  display: flex;
  flex-direction: column;
}

.main-content {
  flex: 1;
  overflow: hidden;
}
</style>

<template>
  <div class="game-details">
    <button @click="backToGames" class="back-btn">← Back to Games</button>
    <div v-if="game" class="details-content">
      <div class="cover-section">
        <img v-if="game.cover" :src="game.cover" alt="Cover" class="large-cover" />
        <div v-else class="placeholder-large-cover">
          {{ game.name.charAt(0).toUpperCase() }}
        </div>
      </div>
      <div class="info-section">
        <h1 class="game-title">{{ game.name }}</h1>
        <p class="game-path">{{ game.path }}</p>
        <p class="game-core">Core: {{ game.core }}</p>
        <div class="actions">
          <button v-if="!isCoreDownloaded" @click="downloadCore" class="btn btn-primary">Download Core</button>
          <button v-else @click="playGame" class="btn btn-success">Play</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useUIStore } from '../stores/ui'
import { useGamesStore } from '../stores/games'
import { useCoresStore } from '../stores/cores'
import { useToast } from '../composables/useToast'
import { LibretroApplication } from '../generated/libretroApplication'

const uiStore = useUIStore()
const gamesStore = useGamesStore()
const coresStore = useCoresStore()
const { addToast } = useToast()

const game = computed(() =>
  uiStore.selectedGameId ? gamesStore.getGame(uiStore.selectedGameId) : null
)

const isCoreDownloaded = computed(() => {
  if (!game.value) return false
  const core = coresStore.cores.find(c => c.name === game.value?.core)
  return core?.status.value === 'downloaded'
})

const backToGames = () => {
  uiStore.backToGames()
}

const downloadCore = async () => {
  if (!game.value) return
  try {
    await coresStore.downloadCore(game.value.core)
    addToast(`Downloaded core: ${game.value.core}`, 'success')
  } catch (e) {
    addToast('Error downloading core: ' + e, 'error')
  }
}

const playGame = async () => {
  if (!game.value) return
  try {
    await LibretroApplication.LoadCore(game.value.core)
    await LibretroApplication.LoadGame(game.value.path)
    await LibretroApplication.Run()
    coresStore.isRunning = true
    uiStore.setView('playing')
    addToast('Game started!', 'success')
  } catch (e) {
    addToast('Error starting game: ' + e, 'error')
  }
}
</script>

<style scoped>
.game-details {
  padding: 2rem;
  height: 100%;
  overflow-y: auto;
}

.back-btn {
  background: none;
  border: none;
  color: #00d4aa;
  font-size: 1rem;
  cursor: pointer;
  margin-bottom: 1rem;
}

.details-content {
  display: flex;
  gap: 2rem;
}

.cover-section {
  flex-shrink: 0;
}

.large-cover {
  width: 300px;
  height: 400px;
  object-fit: cover;
  border-radius: 8px;
}

.placeholder-large-cover {
  width: 300px;
  height: 400px;
  background: #333;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 8rem;
  color: #666;
  border-radius: 8px;
}

.info-section {
  flex: 1;
}

.game-title {
  font-size: 2rem;
  margin-bottom: 0.5rem;
  color: white;
}

.game-path {
  color: #aaa;
  margin-bottom: 0.5rem;
}

.game-core {
  color: #ccc;
  margin-bottom: 1rem;
}

.actions {
  display: flex;
  gap: 1rem;
}
</style>
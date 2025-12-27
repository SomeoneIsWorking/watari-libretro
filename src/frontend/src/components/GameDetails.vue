<template>
  <div class="game-details">
    <button @click="backToGames" class="back-btn">← Back to Games</button>
    <div v-if="game" class="details-content">
      <div class="cover-section">
        <img v-if="game.Cover" :src="game.Cover" alt="Cover" class="large-cover" />
        <div v-else class="placeholder-large-cover">
          {{ game.Name.charAt(0).toUpperCase() }}
        </div>
      </div>
      <div class="info-section">
        <div class="title-section">
          <h1 v-if="!isEditing" class="game-title">{{ game.Name }}</h1>
          <input v-else v-model="newName" class="game-title-input" @keyup.enter="saveRename" />
          <button v-if="!isEditing" @click="startRename" class="rename-btn">✏️</button>
          <div v-else class="rename-actions">
            <button @click="saveRename" class="btn btn-success">Save</button>
            <button @click="cancelRename" class="btn btn-secondary">Cancel</button>
          </div>
        </div>
        <p class="game-path">{{ game.Path }}</p>
        <div class="actions">
          <h3>Available Cores:</h3>
          <div v-for="core in availableCores" :key="core.id" class="core-item">
            <button v-if="core.status.value === 'downloaded'" @click="playGame(core.id)" class="btn btn-success">Play with {{ core.name }}</button>
            <button v-else @click="downloadCore(core.id)" class="btn btn-primary">Download {{ core.name }}</button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue'
import { useUIStore } from '../stores/ui'
import { useCoresStore } from '../stores/cores'
import { useGamesStore } from '../stores/games'
import { useToast } from '../composables/useToast'
import { LibretroApplication } from '../generated/libretroApplication'

const uiStore = useUIStore()
const coresStore = useCoresStore()
const gamesStore = useGamesStore()
const { addToast } = useToast()

const isEditing = ref(false)
const newName = ref('')

const game = computed(() =>
  uiStore.selectedGame
)

const availableCores = computed(() => {
  if (!game.value) return []
  return coresStore.cores.filter(c => {
    return c.database.includes(game.value!.SystemName)
  })
})


const backToGames = () => {
  uiStore.backToGames()
}

const startRename = () => {
  isEditing.value = true
  newName.value = game.value!.Name
}

const saveRename = async () => {
  if (newName.value && newName.value !== game.value!.Name) {
    try {
      await LibretroApplication.RenameGame(game.value!.Path, newName.value)
      uiStore.selectedGame!.Name = newName.value
      const gameInList = gamesStore.games.find(g => g.Path === game.value!.Path)
      if (gameInList) {
        gameInList.Name = newName.value
      }
      addToast('Game renamed successfully', 'success')
    } catch (e) {
      addToast('Error renaming game: ' + e, 'error')
    }
  }
  isEditing.value = false
}

const cancelRename = () => {
  isEditing.value = false
}

const downloadCore = async (coreId: string) => {
  if (!game.value) return
  const core = coresStore.cores.find(c => c.id === coreId)
  if (!core) return
  try {
    await coresStore.downloadCore(coreId)
    addToast(`Downloaded core: ${core.name}`, 'success')
  } catch (e) {
    addToast('Error downloading core: ' + e, 'error')
  }
}

const playGame = async (coreId: string) => {
  if (!game.value) return
  const core = coresStore.cores.find(c => c.id === coreId)
  if (!core) return
  try {
    await LibretroApplication.LoadCore(coreId)
    await LibretroApplication.LoadGame(game.value.Path)
    await LibretroApplication.Run()
    coresStore.isRunning = true
    uiStore.setView('playing')
    addToast(`Game started with ${core.name}!`, 'success')
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

.title-section {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  margin-bottom: 0.5rem;
}

.game-title {
  font-size: 2rem;
  margin: 0;
  color: white;
}

.game-title-input {
  font-size: 2rem;
  background: #333;
  border: 1px solid #555;
  color: white;
  padding: 0.5rem;
  border-radius: 4px;
  flex: 1;
}

.rename-btn {
  background: none;
  border: none;
  color: #00d4aa;
  font-size: 1.5rem;
  cursor: pointer;
}

.rename-btn:hover {
  color: #00b894;
}

.rename-actions {
  display: flex;
  gap: 0.5rem;
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
  flex-direction: column;
  gap: 1rem;
}

.core-item {
  margin-bottom: 0.5rem;
}
</style>
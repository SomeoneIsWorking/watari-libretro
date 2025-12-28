<template>
  <div class="game-details">
    <button @click="backToGames" class="back-btn">← Back to Games</button>
    <div v-if="game" class="details-content">
      <div class="cover-section">
        <img v-if="coverData" :src="coverData" alt="Cover" class="large-cover" />
        <div v-else class="placeholder-large-cover">
          {{ game.Name.charAt(0).toUpperCase() }}
        </div>
      </div>
      <div class="info-section">
        <div class="title-section">
          <h1 v-if="!isEditing" class="game-title">{{ game.Name }}</h1>
          <input v-else v-model="newName" class="game-title-input" @keyup.enter="saveRename" />
          <button v-if="!isEditing" @click="startRename" class="rename-btn">
            <Pencil class="icon-small" />
          </button>
          <div v-else class="rename-actions">
            <button @click="saveRename" class="btn btn-success">Save</button>
            <button @click="cancelRename" class="btn btn-secondary">Cancel</button>
          </div>
        </div>
        <p class="game-path">{{ game.Path }}</p>
        <div class="actions">
          <h3>Available Cores:</h3>
          <div class="cores-grid">
            <div v-if="downloadedCores.length" class="core-row">
              <h4 class="core-subtitle">Downloaded:</h4>
              <div class="core-buttons">
                <button v-for="core in downloadedCores" :key="core.id" @click="playGame(core.id)" class="btn btn-success">Play with {{ core.name }}</button>
              </div>
            </div>
            <div v-if="notDownloadedCores.length" class="core-row">
              <h4 class="core-subtitle">Not Downloaded:</h4>
              <div class="core-buttons">
                <button v-for="core in notDownloadedCores" :key="core.id" @click="downloadCore(core.id)" class="btn btn-primary">Download {{ core.name }}</button>
              </div>
            </div>
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
import { useCover } from '../composables/useCover'
import { LibretroApplication } from '../generated/libretroApplication'
import { Pencil } from 'lucide-vue-next'

const uiStore = useUIStore()
const coresStore = useCoresStore()
const gamesStore = useGamesStore()
const { addToast } = useToast()

const isEditing = ref(false)
const newName = ref('')

const game = computed(() =>
  uiStore.selectedGame
)

const coverData = useCover(game.value?.CoverName || '')

const availableCores = computed(() => {
  if (!game.value) return []
  return coresStore.cores.filter(c => {
    return c.database.includes(game.value!.SystemName)
  })
})

const downloadedCores = computed(() => availableCores.value.filter(c => c.status.value === 'downloaded'))
const notDownloadedCores = computed(() => availableCores.value.filter(c => c.status.value !== 'downloaded'))


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
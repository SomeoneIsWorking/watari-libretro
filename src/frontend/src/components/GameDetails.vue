<template>
  <div class="game-details">
    <button @click="backToGames" class="back-btn mb-4">← Back</button>
    <div v-if="game" class="details-content">
      <div class="cover-section relative">
        <img v-if="coverData" :src="coverData" alt="Cover" class="large-cover w-[300px] h-[450px] overflow-hidden" />
        <div v-else class="placeholder-large-cover">
          {{ game.Name.charAt(0).toUpperCase() }}
        </div>
        <div class="absolute inset-0 bg-black/5 bg-opacity-50 opacity-0 hover:opacity-100 transition-opacity flex items-start justify-end p-2">
          <button @click="downloadCover" class="text-white bg-black/40 rounded cursor-pointer hover:text-gray-300 p-2 m-2" :disabled="isLoadingCovers">
            <Download v-if="!isLoadingCovers" :size="24" />
            <LoaderCircle v-else class="animate-spin" :size="24" />
          </button>
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
        <div class="flex flex-col gap-4">
          <h3>Available Cores:</h3>
          <div class="cores-grid">
            <div v-if="downloadedCores.length" class="core-row">
              <h4 class="core-subtitle">Downloaded:</h4>
              <div class="core-buttons">
                <div v-for="core in downloadedCores" :key="core.id" class="flex items-center gap-2 mb-2">
                  <button @click="playGame(core.id)" class="btn btn-success flex-1">Play with {{ core.name }}</button>
                  <button @click="openCoreSettings(core)" class="btn btn-secondary p-2">
                    <Settings :size="16" />
                  </button>
                </div>
              </div>
            </div>
            <div v-if="notDownloadedCores.length" class="core-row">
              <h4 class="core-subtitle">Not Downloaded:</h4>
              <div class="core-buttons">
                <div v-for="core in notDownloadedCores" :key="core.id" class="mb-2">
                  <div v-if="core.status.value === 'available'" class="flex items-center gap-2">
                    <button @click="downloadCore(core.id)" class="btn btn-primary flex-1">Download {{ core.name }}</button>
                    <button @click="openCoreSettings(core)" class="btn btn-secondary p-2">
                      <Settings :size="16" />
                    </button>
                  </div>
                  <div v-else-if="core.status.value === 'downloading'" class="flex flex-col gap-2 items-start">
                    <span>Downloading {{ core.name }}...</span>
                    <div class="w-full h-2 bg-gray-200 rounded overflow-hidden">
                      <div class="h-full bg-green-500 transition-all duration-300" :style="{ width: `${core.progress.value}%` }"></div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <CoverSearchModal
      v-model="showSearchModal"
      :covers="coverOptions"
      :initial-name="game?.Name"
      title="Select Game Cover"
      :download="selectCover"
      :is-searching="isSearching"
      @search="handleSearch"
    />

    <CoreSettingsModal v-model="showCoreSettingsModal" :core="selectedCoreForSettings" />
  </div>
</template>

<script setup lang="ts">
import { computed, ref, shallowRef } from 'vue'
import { useUIStore } from '../stores/ui'
import { useCoresStore } from '../stores/cores'
import { useGamesStore } from '../stores/games'
import { useToast } from '../composables/useToast'
import { LibretroApplication } from '../generated/libretroApplication'
import { Pencil, Download, LoaderCircle, Settings } from 'lucide-vue-next'
import type { CoverOption } from '../generated/models'
import CoverSearchModal from './CoverSearchModal.vue'
import CoreSettingsModal from './CoreSettingsModal.vue'
import { useSettingsStore } from '../stores/settings'
import type { Core } from '../data/Core'

const uiStore = useUIStore()
const coresStore = useCoresStore()
const gamesStore = useGamesStore()
const settingsStore = useSettingsStore()
const { addToast } = useToast()

const isEditing = ref(false)
const newName = ref('')
const showSearchModal = ref(false)
const coverOptions = ref<CoverOption[]>([])
const isSearching = ref(false)
const isLoadingCovers = ref(false)
const showCoreSettingsModal = ref(false)
const selectedCoreForSettings = shallowRef<Core | null>(null)

const game = computed(() =>
  uiStore.selectedGame
)

const coverData = computed(() => game.value?.getCoverSrc())

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

const downloadCover = async () => {
  if (!game.value) return
  if (!settingsStore.hasApiKey) {
    addToast('SteamGridDB API key is required to search for covers. Please set it in Settings.', 'warning')
    return
  }
  isLoadingCovers.value = true
  try {
    const options = await LibretroApplication.SearchCovers(game.value.Name)
    coverOptions.value = options
    showSearchModal.value = true
  } catch (e) {
    addToast('Error searching covers: ' + e, 'error')
  } finally {
    isLoadingCovers.value = false
  }
}

const handleSearch = async (term: string) => {
  isSearching.value = true
  try {
    const options = await LibretroApplication.SearchCovers(term)
    coverOptions.value = options
  } catch (e) {
    addToast('Error searching covers: ' + e, 'error')
  } finally {
    isSearching.value = false
  }
}

const selectCover = async (fullUrl: string) => {
  if (!game.value) return
  try {
    await LibretroApplication.DownloadGameCover(game.value.Path, fullUrl)
    // Update the cover data
    const base64 = await LibretroApplication.GetCover(game.value.CoverName)
    game.value.setCoverData(base64)
    addToast('Cover downloaded successfully', 'success')
  } catch (e) {
    addToast('Error downloading cover: ' + e, 'error')
  }
}

const openCoreSettings = (core: Core) => {
  selectedCoreForSettings.value = core
  showCoreSettingsModal.value = true
}
</script>
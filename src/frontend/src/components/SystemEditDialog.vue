<template>
  <Dialog v-model="isOpen" title="System Settings" contentClass="overflow-y-auto">
    <div class="flex gap-4">
      <div class="flex-1 space-y-4">
        <div>
          <h4 class="text-accent mb-2">Cores for {{ system.Name }}</h4>
          <div class="cores-list space-y-2">
            <div v-for="core in compatibleCores" :key="core.Id" class="core-item">
              <div class="flex justify-between items-center">
                <span>{{ core.Name }}</span>
                <div class="flex gap-2">
                  <button
                    v-if="!core.IsDownloaded && !downloadingCores.has(core.Id)"
                    @click="downloadCore(core.Id)"
                    class="btn btn-primary btn-sm"
                  >
                    Download
                  </button>
                  <div v-else-if="downloadingCores.has(core.Id)" class="flex items-center gap-2">
                    <progress :value="downloadProgress[core.Id] || 0" max="100" class="flex-1 h-2"></progress>
                    <span class="text-sm">{{ Math.round(downloadProgress[core.Id] || 0) }}%</span>
                  </div>
                  <button
                    v-else
                    @click="removeCore(core.Id)"
                    class="btn btn-secondary btn-sm"
                  >
                    Remove
                  </button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
      <div class="cover-section">
        <div class=" w-[200px] h-[300px] relative large-cover overflow-hidden">
          <img v-if="system.getCoverSrc()" :src="system.getCoverSrc()" alt="Cover" />
          <div v-else class="placeholder-large-cover">
            {{ system.Name.charAt(0).toUpperCase() }}
          </div>
          <div class="absolute inset-0 bg-black/20 opacity-0 hover:opacity-100 transition-opacity flex items-start justify-end p-2 rounded">
            <button @click="downloadCover" class="text-white bg-black/40 rounded cursor-pointer hover:text-gray-300 p-2 m-2" :disabled="isLoadingCovers">
              <Download v-if="!isLoadingCovers" :size="24" />
              <LoaderCircle v-else class="animate-spin" :size="24" />
            </button>
          </div>
        </div>
      </div>
    </div>
  </Dialog>

  <CoverSearchModal
    v-model="showCoverModal"
    :covers="coverOptions"
    :initial-name="system.Name"
    title="Select System Cover"
    :download="selectCover"
    :is-searching="isSearching"
    @search="handleCoverSearch"
  />
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted } from 'vue'
import Dialog from './Dialog.vue'
import CoverSearchModal from './CoverSearchModal.vue'
import { LibretroApplication } from '../generated/libretroApplication'
import type { CoreInfo, CoverOption } from '../generated/models'
import { useSettingsStore } from '../stores/settings'
import { useToast } from '../composables/useToast'
import type { System } from '../data/System'
import { Download, LoaderCircle } from 'lucide-vue-next'

const props = defineProps<{
  system: System
  modelValue: boolean
}>()

const emit = defineEmits<{
  'update:modelValue': [value: boolean]
}>()

const isOpen = computed({
  get: () => props.modelValue,
  set: (value) => emit('update:modelValue', value)
})

const allCores = ref<CoreInfo[]>([])
const downloadingCores = ref<Set<string>>(new Set())
const downloadProgress = ref<Record<string, number>>({})
const showCoverModal = ref(false)
const coverOptions = ref<CoverOption[]>([])
const isSearching = ref(false)
const isLoadingCovers = ref(false)
const settingsStore = useSettingsStore()
const { addToast } = useToast()

const compatibleCores = computed(() => {
  return allCores.value.filter(core =>
    core.Database.includes(props.system.Name)
  )
})

const loadCores = async () => {
  allCores.value = await LibretroApplication.ListCoreInfos()
}

const downloadCore = async (coreId: string) => {
  downloadingCores.value.add(coreId)
  downloadProgress.value[coreId] = 0
  try {
    await LibretroApplication.DownloadCore(coreId)
    await loadCores() // Refresh to update IsDownloaded
  } finally {
    downloadingCores.value.delete(coreId)
    delete downloadProgress.value[coreId]
  }
}

const removeCore = async (coreId: string) => {
  await LibretroApplication.RemoveCore(coreId)
  await loadCores() // Refresh to update IsDownloaded
}

const downloadCover = async () => {
  if (!settingsStore.hasApiKey) {
    addToast('SteamGridDB API key is required to search for covers. Please set it in Settings.', 'warning')
    return
  }
  isLoadingCovers.value = true
  try {
    const options = await LibretroApplication.SearchCovers(props.system.Name)
    coverOptions.value = options
    showCoverModal.value = true
  } catch (e) {
    addToast('Error searching covers: ' + e, 'error')
  } finally {
    isLoadingCovers.value = false
  }
}

const handleCoverSearch = async (term: string) => {
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
  try {
    await LibretroApplication.DownloadSystemCover(props.system.Name, fullUrl)
    const base64 = await LibretroApplication.GetCover(props.system.CoverName)
    props.system.setCoverData(base64)
    addToast('Cover downloaded successfully', 'success')
  } catch (e) {
    addToast('Error downloading cover: ' + e, 'error')
  }
}

let progressUnsub: (() => void) | null = null
let completeUnsub: (() => void) | null = null

onMounted(() => {
  loadCores()
  progressUnsub = LibretroApplication.OnDownloadProgress((data) => {
    downloadProgress.value[data.Name] = data.Progress
  })
  completeUnsub = LibretroApplication.OnDownloadComplete((name) => {
    downloadingCores.value.delete(name)
    delete downloadProgress.value[name]
    loadCores()
  })
})

onUnmounted(() => {
  if (progressUnsub) progressUnsub()
  if (completeUnsub) completeUnsub()
})
</script>
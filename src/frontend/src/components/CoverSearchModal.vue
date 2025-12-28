<template>
  <Dialog v-model="modelValue" :title="title" class="h-[80vh]" contentClass="overflow-hidden">
    <div class="flex flex-col h-full">
      <div class="mb-3 flex-shrink-0">
        <div class="flex gap-2">
          <input
            v-model="searchTerm"
            type="text"
            placeholder="Search for covers..."
            class="flex-1 px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
            @keyup.enter="handleSearch"
          />
          <LoadingButton
            :loading="props.isSearching"
            text="Search"
            loading-text="Searching..."
            variant="primary"
            @click="handleSearch"
          />
        </div>
      </div>
      <div class="flex-1 overflow-y-auto -mb-4 pb-4 pr-4 -mr-4">
        <div class="grid grid-cols-6 gap-4">
          <div
            v-for="cover in covers"
            :key="cover.Id"
            class="cursor-pointer border border-gray-400 rounded hover:border-white dark:hover:bg-gray-600 transition-colors relative"
            @click="handleSelect(cover.Id)"
          >
            <img :src="cover.ThumbUrl" :alt="cover.Name" class="w-40 h-60 object-cover rounded" />
            <div v-if="downloading === cover.Id" class="absolute inset-0 bg-black bg-opacity-50 flex items-center justify-center rounded">
              <div class="text-white text-xs">Downloading...</div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </Dialog>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue'
import Dialog from './Dialog.vue'
import LoadingButton from './LoadingButton.vue'
import type { CoverOption } from '../generated/models'
import { useSettingsStore } from '../stores/settings'
import { useToast } from '../composables/useToast'

const props = defineProps<{
  title: string
  covers: CoverOption[]
  initialName?: string
  download: (fullUrl: string) => Promise<void>
  isSearching?: boolean
}>()

const modelValue = defineModel<boolean>()

const emit = defineEmits<{
  search: [term: string]
}>()

const settingsStore = useSettingsStore()
const { addToast } = useToast()

const searchTerm = ref(props.initialName || '')
const downloading = ref<string | null>(null)

watch(() => props.initialName, (newVal) => {
  searchTerm.value = newVal || ''
})

const handleSelect = async (id: string) => {
  const option = props.covers.find(c => c.Id === id)
  if (!option) return
  downloading.value = id
  try {
    await props.download(option.FullUrl)
    modelValue.value = false
  } catch (e) {
    addToast('Error downloading cover: ' + e, 'error')
  } finally {
    downloading.value = null
  }
}

const handleSearch = () => {
  if (!settingsStore.hasApiKey) {
    addToast('SteamGridDB API key is required to search for covers. Please set it in Settings.', 'warning')
    return
  }
  if (searchTerm.value.trim()) {
    emit('search', searchTerm.value.trim())
  }
}
</script>
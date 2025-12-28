<template>
  <Dialog v-model="modelValue" title="Settings">
    <form @submit.prevent="saveSettings">
      <div class="mb-4">
        <label for="apiKey" class="block text-sm font-medium mb-2">SteamGridDB API Key</label>
        <input
          id="apiKey"
          v-model="apiKey"
          type="password"
          placeholder="Enter your SteamGridDB API key"
          class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
        />
        <p class="text-sm text-gray-600 dark:text-gray-400 mt-1">
          Get your API key from <a href="https://www.steamgriddb.com/profile/preferences/api" target="_blank" class="text-blue-500">SteamGridDB</a>
        </p>
      </div>
      <div class="flex justify-end gap-2">
        <button type="button" @click="modelValue = false" class="btn btn-secondary">Cancel</button>
        <button type="submit" class="btn btn-primary">Save</button>
      </div>
    </form>
  </Dialog>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue'
import Dialog from './Dialog.vue'
import { useSettingsStore } from '../stores/settings'
import { useToast } from '../composables/useToast'

const { addToast } = useToast()
const settingsStore = useSettingsStore()

const modelValue = defineModel<boolean>()

const apiKey = ref('')

const loadSettings = async () => {
  await settingsStore.loadSettings()
  apiKey.value = settingsStore.steamGridDBApiKey
}

watch(modelValue, (newVal) => {
  if (newVal) {
    loadSettings()
  }
})

const saveSettings = async () => {
  try {
    settingsStore.steamGridDBApiKey = apiKey.value
    await settingsStore.saveSettings()
    addToast('Settings saved successfully', 'success')
    modelValue.value = false
  } catch (e) {
    addToast('Error saving settings: ' + e, 'error')
  }
}
</script>
import { ref, computed } from 'vue'
import { defineStore } from 'pinia'
import { LibretroApplication } from '../generated/libretroApplication'

export const useSettingsStore = defineStore('settings', () => {
  const steamGridDBApiKey = ref('')
  const isLoaded = ref(false)

  const hasApiKey = computed(() => !!steamGridDBApiKey.value.trim())

  const loadSettings = async () => {
    const settings = await LibretroApplication.GetSettings()
    steamGridDBApiKey.value = settings.SteamGridDBApiKey || ''
    isLoaded.value = true
  }

  const saveSettings = async () => {
    await LibretroApplication.SaveSettings({ SteamGridDBApiKey: steamGridDBApiKey.value })
  }

  return {
    steamGridDBApiKey,
    isLoaded,
    hasApiKey,
    loadSettings,
    saveSettings
  }
})
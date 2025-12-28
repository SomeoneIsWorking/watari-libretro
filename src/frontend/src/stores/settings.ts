import { ref, computed } from 'vue'
import { defineStore } from 'pinia'
import { LibretroApplication } from '../generated/libretroApplication'

export const useSettingsStore = defineStore('settings', () => {
  const steamGridDBApiKey = ref('')
  const isLoaded = ref(false)

  const hasApiKey = computed(() => !!steamGridDBApiKey.value.trim())

  const loadSettings = async () => {
    try {
      const settings = await LibretroApplication.GetSettings()
      steamGridDBApiKey.value = settings.SteamGridDBApiKey || ''
      isLoaded.value = true
    } catch (e) {
      console.error('Error loading settings:', e)
    }
  }

  const saveSettings = async () => {
    try {
      await LibretroApplication.SaveSettings({ SteamGridDBApiKey: steamGridDBApiKey.value })
    } catch (e) {
      console.error('Error saving settings:', e)
      throw e
    }
  }

  return {
    steamGridDBApiKey,
    isLoaded,
    hasApiKey,
    loadSettings,
    saveSettings
  }
})
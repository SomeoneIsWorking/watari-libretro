import { ref } from 'vue'
import { defineStore } from 'pinia'
import { LibretroApplication } from '../generated/libretroApplication'
import { useToast } from '../composables/useToast'
import { useCoresStore } from './cores'

export const useGameStore = defineStore('game', () => {
  const { addToast } = useToast()
  const coresStore = useCoresStore()

  const romPath = ref<string>('')
  const isGameLoaded = ref(false)
  const status = ref<string>('')

  const selectRomFile = async () => {
    try {
      const path = await watari.openFileDialog('nes,snes,gb,gba,zip,7z')
      if (path) {
        romPath.value = path
        status.value = `Selected ROM: ${path}`
        addToast(`Selected ROM: ${path}`, 'info')
      }
    } catch (e) {
      addToast('Error selecting file: ' + e, 'error')
    }
  }

  const loadGame = async () => {
    if (!romPath.value) {
      addToast('Please select a ROM file', 'error')
      return
    }
    try {
      await LibretroApplication.LoadGame(romPath.value)
      status.value = `Loaded game: ${romPath.value}`
      addToast(`Loaded game: ${romPath.value}`, 'success')
      isGameLoaded.value = true
    } catch (e) {
      addToast('Error loading game: ' + e, 'error')
      throw e
    }
  }

  const run = async () => {
    try {
      await LibretroApplication.Run()
      coresStore.isRunning = true
      status.value = 'Game running'
      addToast('Game running', 'success')
    } catch (e) {
      addToast('Error running: ' + e, 'error')
      throw e
    }
  }

  return {
    romPath,
    isGameLoaded,
    status,
    selectRomFile,
    loadGame,
    run
  }
})
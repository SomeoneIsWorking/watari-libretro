import { ref } from 'vue'
import { defineStore } from 'pinia'
import type { GameInfo, SystemInfo } from '../generated/models'

export type View = 'systems' | 'games' | 'details' | 'playing'

export const useUIStore = defineStore('ui', () => {
  const currentView = ref<View>('systems')
  const selectedGame = ref<GameInfo | null>(null)
  const currentSystem = ref<SystemInfo | null>(null)
  const isMenuOpen = ref(false)
  const searchQuery = ref('')

  const setView = (view: View) => {
    currentView.value = view
  }

  const selectSystem = (system: SystemInfo) => {
    currentSystem.value = system
    currentView.value = 'games'
  }

  const selectGame = (game: GameInfo) => {
    selectedGame.value = game
    if (game) {
      currentView.value = 'details'
    }
  }

  const backToSystems = () => {
    currentView.value = 'systems'
    currentSystem.value = null
    selectedGame.value = null
  }

  const backToGames = () => {
    currentView.value = 'games'
    selectedGame.value = null
  }

  const toggleMenu = () => {
    isMenuOpen.value = !isMenuOpen.value
  }

  return {
    currentView,
    selectedGame,
    currentSystem,
    isMenuOpen,
    searchQuery,
    setView,
    selectSystem,
    selectGame,
    backToSystems,
    backToGames,
    toggleMenu,
  }
})
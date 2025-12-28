import { ref, shallowRef } from 'vue'
import { defineStore } from 'pinia'
import { Game } from '../data/Game'
import { System } from '../data/System'

export type View = 'systems' | 'games' | 'details' | 'playing'

export const useUIStore = defineStore('ui', () => {
  const currentView = ref<View>('systems')
  const selectedGame = shallowRef<Game | null>(null)
  const currentSystem = shallowRef<System | null>(null)
  const isMenuOpen = ref(false)
  const searchQuery = ref('')

  const setView = (view: View) => {
    currentView.value = view
  }

  const selectSystem = (system: System | null) => {
    currentSystem.value = system
    currentView.value = 'games'
  }

  const selectGame = (game: Game) => {
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
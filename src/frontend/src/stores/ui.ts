import { ref } from 'vue'
import { defineStore } from 'pinia'

export type View = 'systems' | 'games' | 'details' | 'playing'

export const useUIStore = defineStore('ui', () => {
  const currentView = ref<View>('systems')
  const selectedGameId = ref<string | null>(null)
  const currentSystem = ref<string | null>(null)
  const isMenuOpen = ref(false)
  const searchQuery = ref('')

  const setView = (view: View) => {
    currentView.value = view
  }

  const selectSystem = (system: string) => {
    currentSystem.value = system
    currentView.value = 'games'
  }

  const selectGame = (gameId: string | null) => {
    selectedGameId.value = gameId
    if (gameId) {
      currentView.value = 'details'
    }
  }

  const backToSystems = () => {
    currentView.value = 'systems'
    currentSystem.value = null
    selectedGameId.value = null
  }

  const backToGames = () => {
    currentView.value = 'games'
    selectedGameId.value = null
  }

  const toggleMenu = () => {
    isMenuOpen.value = !isMenuOpen.value
  }

  const setSearchQuery = (query: string) => {
    searchQuery.value = query
  }

  return {
    currentView,
    selectedGameId,
    currentSystem,
    isMenuOpen,
    searchQuery,
    setView,
    selectSystem,
    selectGame,
    backToSystems,
    backToGames,
    toggleMenu,
    setSearchQuery
  }
})
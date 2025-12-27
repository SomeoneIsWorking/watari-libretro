import { ref, computed } from 'vue'
import { defineStore } from 'pinia'
import { LibretroApplication } from '../generated/libretroApplication'
import type { GameInfo, SystemInfo } from '../generated/models'

export const useGamesStore = defineStore('games', () => {
  const games = ref<GameInfo[]>([])
  const systems = ref<SystemInfo[]>([])

  const getSystemsWithGames = computed(() => {
    const systemNames = [...new Set(games.value.map(g => g.SystemName))]
    return systems.value.filter(s => systemNames.includes(s.Name))
  })

  const loadSystems = async () => {
    systems.value = await LibretroApplication.GetSystems()
  }

  const loadLibrary = async () => {
    await loadSystems()
    games.value = await LibretroApplication.LoadLibrary()
  }

  const removeGame = async (path: string) => {
    await LibretroApplication.RemoveGame(path)
    await loadLibrary()
  }

  const getGame = (path: string) => {
    return games.value.find(g => g.Path === path)
  }

  return {
    games,
    systems,
    getSystemsWithGames,
    loadLibrary,
    loadSystems,
    removeGame,
    getGame,
  }
})
import { ref, computed } from 'vue'
import { defineStore } from 'pinia'
import { LibretroApplication } from '../generated/libretroApplication'

export interface Game {
  name: string
  path: string
  systemId: string
  cover?: string
}

export interface System {
  name: string
  defaultCore: string
  extensions: string[]
}

export const useGamesStore = defineStore('games', () => {
  const games = ref<Game[]>([])
  const systems = ref<System[]>([])

  const getSystemForExtension = (ext: string) => {
    return systems.value.find(s => s.extensions.includes(ext.toLowerCase()))
  }

  const getGamesForSystem = (systemId: string) => {
    return games.value.filter(g => g.systemId === systemId)
  }

  const getSystemsWithGames = computed(() => {
    const systemIds = [...new Set(games.value.map(g => g.systemId))]
    return systems.value.filter(s => systemIds.includes(s.name))
  })

  const loadSystems = async () => {
    try {
      const sys = await LibretroApplication.GetSystems()
      systems.value = sys.map(s => ({
        name: s.Name,
        defaultCore: s.DefaultCore,
        extensions: s.Extensions
      }))
    } catch (e) {
      console.error('Error loading systems:', e)
    }
  }

  const loadLibrary = async () => {
    try {
      await loadSystems()
      const library = await LibretroApplication.LoadLibrary()
      games.value = library.map(g => {
        const detectedSystemId = detectSystem(g.Path)
        const systemId = (!g.SystemId || g.SystemId === "unknown") ? detectedSystemId : g.SystemId
        return {
          name: g.Name,
          path: g.Path,
          systemId,
          cover: g.Cover || undefined
        }
      })
    } catch (e) {
      console.error('Error loading library:', e)
    }
  }

  const detectSystem = (path: string) => {
    const ext = path.split('.').pop()?.toLowerCase() || ''
    const system = getSystemForExtension(ext)
    return system ? system.name : 'unknown'
  }

  const removeGame = async (path: string) => {
    await LibretroApplication.RemoveGame(path)
    await loadLibrary()
  }

  const getGame = (path: string) => {
    return games.value.find(g => g.path === path)
  }

  const getFilteredGames = (query: string, systemId?: string) => {
    let filtered = systemId ? getGamesForSystem(systemId) : games.value
    if (query) {
      filtered = filtered.filter(g => g.name.toLowerCase().includes(query.toLowerCase()))
    }
    return filtered
  }

  return {
    games,
    systems,
    getSystemsWithGames,
    loadLibrary,
    loadSystems,
    removeGame,
    getGame,
    getFilteredGames
  }
})
import { computed } from 'vue'
import { defineStore } from 'pinia'
import { LibretroApplication } from '../generated/libretroApplication'
import { Game } from '../data/Game'
import { System } from '../data/System'
import { shallowArray } from '../util/shallowArray'

export const useGamesStore = defineStore('games', () => {
  const games = shallowArray<Game>([])
  const systems = shallowArray<System>([])

  const getSystemsWithGames = computed(() => {
    const systemNames = [...new Set(games.value.map(g => g.SystemName))]
    return systems.value.filter(s => systemNames.includes(s.Name))
  })

  const loadSystems = async () => {
    const systemInfos = await LibretroApplication.GetSystems()
    systems.value = await Promise.all(systemInfos.map(async s => {
      const system = new System(s.Name, s.Extensions, s.CoverName)
      const coverData = await LibretroApplication.GetCover(s.CoverName)
      system.setCoverData(coverData)
      return system
    }))
  }

  const loadLibrary = async () => {
    await loadSystems()
    const gameInfos = await LibretroApplication.LoadLibrary()
    games.value = await Promise.all(gameInfos.map(async g => {
      const game = new Game(g.Path, g.Name, g.SystemName, g.CoverName)
      const coverData = await LibretroApplication.GetCover(g.CoverName)
      game.setCoverData(coverData)
      return game
    }))
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
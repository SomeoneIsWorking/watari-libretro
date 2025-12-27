import { ref } from 'vue'
import { defineStore } from 'pinia'
import { LibretroApplication } from '../generated/libretroApplication'
import { Core } from '../data/Core'
import { shallowReactiveRef as shallowArray } from '../util/shallowReactiveRef'

export const useCoresStore = defineStore('cores', () => {
  const cores = shallowArray<Core>([])
  const selectedCore = ref<string>('')
  const isRunning = ref(false)
  const isMenuOpen = ref(false)

  const initialize = async () => {
    try {
      const availableCores = await LibretroApplication.ListCoreInfos()
      cores.value = availableCores.map(c => new Core(c.Name, c.Status === 'downloaded'))
    } catch (e) {
      console.error('Error loading cores:', e)
      throw e
    }
  }

  const downloadCore = async (name: string) => {
    const core = cores.value.find(c => c.name === name)
    if (!core) return

    core.setDownloading()
    try {
      await LibretroApplication.DownloadCore(name)
      core.setDownloaded()
    } catch (e) {
      console.error('Error downloading core:', e)
      core.setError()
      throw e
    }
  }

  const loadCore = async (name: string) => {
    const core = cores.value.find(c => c.name === name)
    if (!core) return

    try {
      await LibretroApplication.LoadCore(name)
      selectedCore.value = name
    } catch (e) {
      console.error('Error loading core:', e)
      throw e
    }
  }

  const getCoreList = () => cores.value

  return {
    cores,
    selectedCore,
    isRunning,
    isMenuOpen,
    initialize,
    downloadCore,
    loadCore,
    getCoreList
  }
})
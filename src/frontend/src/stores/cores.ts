import { ref } from 'vue'
import { defineStore } from 'pinia'
import { LibretroApplication } from '../generated/libretroApplication'
import { Core } from '../data/Core'
import { shallowArray } from '../util/shallowArray'
import type { DownloadProgress } from '../generated/models'

export const useCoresStore = defineStore('cores', () => {
  const cores = shallowArray<Core>([])
  const selectedCore = ref<string>('')
  const isRunning = ref(false)
  const isMenuOpen = ref(false)

  let progressUnsub: (() => void) | null = null

  const initialize = async () => {
    try {
      const availableCores = await LibretroApplication.ListCoreInfos()
      cores.value = availableCores.map(c => new Core(c.Id, c.Name, c.Database, c.IsDownloaded))

      if (!progressUnsub) {
        progressUnsub = LibretroApplication.OnDownloadProgress((data: DownloadProgress) => {
          const core = cores.value.find(c => c.id === data.Name)
          if (core && core.status.value === 'downloading') {
            core.setProgress(data.Progress)
          }
        })
      }
    } catch (e) {
      console.error('Error loading cores:', e)
      throw e
    }
  }

  const downloadCore = async (id: string) => {
    const core = cores.value.find(c => c.id === id)
    if (!core) return

    core.setDownloading()
    try {
      await LibretroApplication.DownloadCore(id)
      core.setDownloaded()
    } catch (e) {
      console.error('Error downloading core:', e)
      core.setError()
      throw e
    }
  }

  const loadCore = async (id: string) => {
    const core = cores.value.find(c => c.id === id)
    if (!core) return

    try {
      await LibretroApplication.LoadCore(id)
      selectedCore.value = id
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
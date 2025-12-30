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
  let initPromise: Promise<void> | null = null

  const initialize = async () => {
    if (initPromise) return initPromise

    initPromise = (async () => {
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
    })()

    return initPromise
  }

  const downloadCore = async (id: string) => {
    const core = cores.value.find(c => c.id === id)
    if (!core) return

    core.setDownloading()
    await LibretroApplication.DownloadCore(id)
    core.setDownloaded()
  }

  const loadCore = async (id: string) => {
    const core = cores.value.find(c => c.id === id)
    if (!core) return

    await LibretroApplication.LoadCore(id)
    selectedCore.value = id
  }

  const removeCore = async (id: string) => {
    const core = cores.value.find(c => c.id === id)
    if (!core) return

    await LibretroApplication.RemoveCore(id)
    core.setRemoved()
  }

  const getCoreList = () => cores.value

  return {
    cores,
    selectedCore,
    isRunning,
    isMenuOpen,
    initialize,
    downloadCore,
    removeCore,
    loadCore,
    getCoreList
  }
})
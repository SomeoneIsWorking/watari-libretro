import { ref, onMounted, watch } from 'vue'
import { LibretroApplication } from '../generated/libretroApplication'

export function useCover(coverPath: string) {
  const coverData = ref<string | null>(null)

  const loadCover = async () => {
    if (coverPath) {
      try {
        const base64 = await LibretroApplication.GetCover(coverPath)
        if (base64) {
          coverData.value = `data:image/png;base64,${base64}`
        } else {
          coverData.value = null
        }
      } catch (error) {
        console.error('Failed to load cover:', error)
        coverData.value = null
      }
    } else {
      coverData.value = null
    }
  }

  onMounted(() => {
    loadCover()
  })

  watch(() => coverPath, () => {
    loadCover()
  })

  return coverData
}
<template>
  <Dialog v-model="isOpen" title="System Settings">
    <div class="space-y-4">
      <div>
        <h4 class="text-accent mb-2">Cores for {{ system.Name }}</h4>
        <div class="cores-list space-y-2">
          <div v-for="core in compatibleCores" :key="core.Id" class="core-item">
            <div class="flex justify-between items-center">
              <span>{{ core.Name }}</span>
              <div class="flex gap-2">
                <button
                  v-if="!core.IsDownloaded && !downloadingCores.has(core.Id)"
                  @click="downloadCore(core.Id)"
                  class="btn btn-primary btn-sm"
                >
                  Download
                </button>
                <div v-else-if="downloadingCores.has(core.Id)" class="flex items-center gap-2">
                  <progress :value="downloadProgress[core.Id] || 0" max="100" class="flex-1 h-2"></progress>
                  <span class="text-sm">{{ Math.round(downloadProgress[core.Id] || 0) }}%</span>
                </div>
                <button
                  v-else
                  @click="removeCore(core.Id)"
                  class="btn btn-secondary btn-sm"
                >
                  Remove
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>
      <div>
        <h4 class="text-accent mb-2">Cover</h4>
        <button class="btn btn-secondary" disabled>Download Cover (TODO)</button>
      </div>
    </div>
  </Dialog>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted } from 'vue'
import Dialog from './Dialog.vue'
import { LibretroApplication } from '../generated/libretroApplication'
import type { SystemInfo, CoreInfo } from '../generated/models'

const props = defineProps<{
  system: SystemInfo
  modelValue: boolean
}>()

const emit = defineEmits<{
  'update:modelValue': [value: boolean]
}>()

const isOpen = computed({
  get: () => props.modelValue,
  set: (value) => emit('update:modelValue', value)
})

const allCores = ref<CoreInfo[]>([])
const downloadingCores = ref<Set<string>>(new Set())
const downloadProgress = ref<Record<string, number>>({})

const compatibleCores = computed(() => {
  return allCores.value.filter(core =>
    core.Database.includes(props.system.Name)
  )
})

const loadCores = async () => {
  allCores.value = await LibretroApplication.ListCoreInfos()
}

const downloadCore = async (coreId: string) => {
  downloadingCores.value.add(coreId)
  downloadProgress.value[coreId] = 0
  try {
    await LibretroApplication.DownloadCore(coreId)
    await loadCores() // Refresh to update IsDownloaded
  } finally {
    downloadingCores.value.delete(coreId)
    delete downloadProgress.value[coreId]
  }
}

const removeCore = async (coreId: string) => {
  await LibretroApplication.RemoveCore(coreId)
  await loadCores() // Refresh to update IsDownloaded
}

let progressUnsub: (() => void) | null = null
let completeUnsub: (() => void) | null = null

onMounted(() => {
  loadCores()
  progressUnsub = LibretroApplication.OnDownloadProgress((data) => {
    downloadProgress.value[data.Name] = data.Progress
  })
  completeUnsub = LibretroApplication.OnDownloadComplete((name) => {
    downloadingCores.value.delete(name)
    delete downloadProgress.value[name]
    loadCores()
  })
})

onUnmounted(() => {
  if (progressUnsub) progressUnsub()
  if (completeUnsub) completeUnsub()
})
</script>
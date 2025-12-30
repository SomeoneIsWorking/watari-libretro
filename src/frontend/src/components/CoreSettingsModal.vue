<template>
  <Dialog v-model="isOpen" :title="core?.name || 'Core Settings'" contentClass="overflow-y-auto">
    <div v-if="core" class="space-y-4">
      <div v-if="!core.isDownloaded" class="text-center">
        <p class="mb-4">This core is not downloaded.</p>
        <button @click="downloadCore" class="btn btn-primary" :disabled="core.status.value === 'downloading'">
          <span v-if="core.status.value === 'downloading'">Downloading...</span>
          <span v-else>Download Core</span>
        </button>
      </div>
      <div v-else>
        <div class="space-y-2">
          <div>
            <strong>Name:</strong> {{ core.name }}
          </div>
          <div>
            <strong>ID:</strong> {{ core.id }}
          </div>
          <div>
            <strong>Databases:</strong> {{ core.database.join(', ') }}
          </div>
        </div>
        <div v-if="coreOptions" class="mt-4">
          <h4 class="font-semibold mb-2">Core Options:</h4>
          <div class="space-y-4">
            <CoreOption
              v-for="(definition, key) in coreOptions"
              :key="key"
              :option-key="key"
              :definition="definition"
              :current-value="currentValues[key] || parseDefault(definition)"
              @change="updateOption"
            />
          </div>
        </div>
        <div class="mt-4 flex justify-end">
          <button @click="deleteCore" class="btn btn-danger">Delete Core</button>
        </div>
      </div>
    </div>
  </Dialog>
</template>

<script setup lang="ts">
import { ref, watch, computed } from 'vue'
import Dialog from './Dialog.vue'
import CoreOption from './CoreOption.vue'
import { LibretroApplication } from '../generated/libretroApplication'
import { useToast } from '../composables/useToast'
import { useCoresStore } from '../stores/cores'
import type { Core } from '../data/Core'

const props = defineProps<{
  core: Core | null
  modelValue: boolean
}>()

const emit = defineEmits<{
  'update:modelValue': [value: boolean]
}>()

const isOpen = computed({
  get: () => props.modelValue,
  set: (value) => emit('update:modelValue', value)
})

const coreOptions = ref<Record<string, string> | null>(null)
const currentValues = ref<Record<string, string>>({})
const { addToast } = useToast()
const coresStore = useCoresStore()

watch(() => props.core, async (newCore) => {
  if (newCore && newCore.isDownloaded) {
    try {
      coreOptions.value = await LibretroApplication.GetCoreOptions(newCore.id)
      currentValues.value = await LibretroApplication.LoadCoreOptionValues(newCore.id)
    } catch (e) {
      addToast('Error loading core options: ' + e, 'error')
      coreOptions.value = null
      currentValues.value = {}
    }
  } else {
    coreOptions.value = null
    currentValues.value = {}
  }
}, { immediate: true })

const downloadCore = async () => {
  if (!props.core) return
  try {
    await coresStore.downloadCore(props.core.id)
    addToast(`Downloaded core: ${props.core.name}`, 'success')
    // Reload options
    coreOptions.value = await LibretroApplication.GetCoreOptions(props.core.id)
  } catch (e) {
    addToast('Error downloading core: ' + e, 'error')
  }
}

const updateOption = async (key: string, value: string) => {
  currentValues.value[key] = value
  try {
    await LibretroApplication.SaveCoreOptionValues(props.core!.id, currentValues.value)
    addToast('Option saved', 'success')
  } catch (e) {
    addToast('Error saving option: ' + e, 'error')
  }
}

const parseDefault = (definition: string): string => {
  const parts = definition.split(';')
  if (parts.length > 2) {
    return parts[2]!.trim()
  }
  const options = definition.split(';')[1]?.split('|').map(o => o.trim()) || []
  return options[0] || ''
}

const deleteCore = async () => {
  if (!props.core) return
  try {
    await coresStore.removeCore(props.core.id)
    addToast(`Deleted core: ${props.core.name}`, 'success')
    coreOptions.value = null
    currentValues.value = {}
    isOpen.value = false
  } catch (e) {
    addToast('Error deleting core: ' + e, 'error')
  }
}
</script>
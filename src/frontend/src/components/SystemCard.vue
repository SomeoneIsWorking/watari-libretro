<template>
  <div class="system-card" @click="selectSystem">
    <div class="system-cover">
      <img v-if="coverData" :src="coverData" alt="Cover" />
      <div v-else class="system-placeholder">
        {{ system.Name.charAt(0) }}
      </div>
      <div class="overlay">
        <h3 class="system-name">{{ system.Name }}</h3>
        <p class="game-count">{{ gameCount }} games</p>
      </div>
      <button @click.stop="openEditDialog" class="edit-btn">
        <Settings class="icon-small" />
      </button>
    </div>
  </div>
  <SystemEditDialog v-model="isEditOpen" :system="system" />
</template>

<script setup lang="ts">
import { computed, ref } from 'vue'
import { useUIStore } from '../stores/ui'
import { useGamesStore } from '../stores/games'
import type { SystemInfo } from '../generated/models'
import { useCover } from '../composables/useCover'
import SystemEditDialog from './SystemEditDialog.vue'
import { Settings } from 'lucide-vue-next'

const props = defineProps<{
  system: SystemInfo
}>()

const uiStore = useUIStore()
const gamesStore = useGamesStore()
const isEditOpen = ref(false)

const coverData = useCover(props.system.CoverName)

const gameCount = computed(() => {
  return gamesStore.games.filter(g => g.SystemName === props.system.Name).length
})

const selectSystem = () => {
  uiStore.selectSystem(props.system)
}

const openEditDialog = () => {
  isEditOpen.value = true
}
</script>
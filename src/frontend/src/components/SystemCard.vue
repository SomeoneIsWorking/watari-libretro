<template>
  <div class="system-card" @click="selectSystem">
    <div class="system-cover">
      <img v-if="coverData" :src="coverData" alt="Cover" />
      <div v-else class="system-placeholder">
        {{ system.Name.charAt(0) }}
      </div>
    </div>
    <h3 class="system-name">{{ system.Name }}</h3>
    <p class="game-count">{{ gameCount }} games</p>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useUIStore } from '../stores/ui'
import { useGamesStore } from '../stores/games'
import type { SystemInfo } from '../generated/models'
import { useCover } from '../composables/useCover'

const props = defineProps<{
  system: SystemInfo
}>()

const uiStore = useUIStore()
const gamesStore = useGamesStore()

const coverData = useCover(props.system.CoverName)

const gameCount = computed(() => {
  return gamesStore.games.filter(g => g.SystemName === props.system.Name).length
})

const selectSystem = () => {
  uiStore.selectSystem(props.system)
}
</script>
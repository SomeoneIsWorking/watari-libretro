<template>
  <div class="systems-grid">
    <div v-if="systemsWithGames.length === 0" class="empty-state">
      <p>No systems with games. Add some games to get started!</p>
    </div>
    <div v-else class="grid">
      <div
        v-for="system in systemsWithGames"
        :key="system.Name"
        class="system-card"
        @click="selectSystem(system)"
      >
        <div class="system-icon">
          {{ system.Name.charAt(0) }}
        </div>
        <h3 class="system-name">{{ system.Name }}</h3>
        <p class="game-count">{{ getGameCount(system.Name) }} games</p>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useUIStore } from '../stores/ui'
import { useGamesStore } from '../stores/games'
import type { SystemInfo } from '../generated/models'

const uiStore = useUIStore()
const gamesStore = useGamesStore()

const systemsWithGames = computed(() => gamesStore.getSystemsWithGames)

const getGameCount = (systemName: string) => {
  return gamesStore.games.filter(g => g.SystemName === systemName).length
}

const selectSystem = (system: SystemInfo) => {
  uiStore.selectSystem(system)
}
</script>
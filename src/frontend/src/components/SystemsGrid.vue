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

<style scoped>
.systems-grid {
  padding: 2rem;
  height: 100%;
  overflow-y: auto;
}

.empty-state {
  display: flex;
  justify-content: center;
  align-items: center;
  height: 100%;
  color: #666;
  font-size: 1.2rem;
}

.grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
  gap: 2rem;
}

.system-card {
  background: #2a2a2a;
  border-radius: 8px;
  padding: 2rem;
  text-align: center;
  cursor: pointer;
  transition: transform 0.2s, box-shadow 0.2s;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.3);
}

.system-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 4px 16px rgba(0, 0, 0, 0.5);
}

.system-icon {
  font-size: 4rem;
  color: #00d4aa;
  margin-bottom: 1rem;
}

.system-name {
  margin: 0 0 0.5rem 0;
  color: white;
  font-size: 1.2rem;
}

.game-count {
  margin: 0;
  color: #aaa;
  font-size: 0.9rem;
}
</style>
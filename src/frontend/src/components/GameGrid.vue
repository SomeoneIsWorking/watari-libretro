<template>
  <div class="game-grid">
    <div class="header">
      <button @click="uiStore.backToSystems" class="back-btn">← Back to Systems</button>
      <h2>{{ uiStore.currentSystem }} Games</h2>
    </div>
    <div v-if="filteredGames.length === 0" class="empty-state">
      <p>No games found for {{ uiStore.currentSystem }}. Add some games!</p>
    </div>
    <div v-else class="grid">
      <GameCard
        v-for="game in filteredGames"
        :key="game.id"
        :game="game"
      />
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useGamesStore } from '../stores/games'
import { useUIStore } from '../stores/ui'
import GameCard from './GameCard.vue'

const gamesStore = useGamesStore()
const uiStore = useUIStore()

const filteredGames = computed(() =>
  gamesStore.getFilteredGames(uiStore.searchQuery, uiStore.currentSystem || undefined)
)
</script>

<style scoped>
.game-grid {
  padding: 2rem;
  height: 100%;
  overflow-y: auto;
}

.header {
  display: flex;
  align-items: center;
  gap: 1rem;
  margin-bottom: 2rem;
}

.back-btn {
  background: none;
  border: none;
  color: #00d4aa;
  font-size: 1rem;
  cursor: pointer;
}

.header h2 {
  margin: 0;
  color: white;
}

.empty-state {
  display: flex;
  justify-content: center;
  align-items: center;
  height: 50vh;
  color: #666;
  font-size: 1.2rem;
}

.grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
  gap: 2rem;
}
</style>
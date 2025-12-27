<template>
  <div class="game-card" @click="selectGame">
    <div class="game-cover">
      <img v-if="game.cover" :src="game.cover" alt="Cover" />
      <div v-else class="placeholder-cover">
        {{ game.name.charAt(0).toUpperCase() }}
      </div>
    </div>
    <div class="game-info">
      <h3 class="game-title">{{ game.name }}</h3>
      <p class="game-core">{{ game.core || 'No core selected' }}</p>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useUIStore } from '../stores/ui'
import type { Game } from '../stores/games'

const props = defineProps<{
  game: Game
}>()

const uiStore = useUIStore()

const selectGame = () => {
  uiStore.selectGame(props.game.id)
}
</script>

<style scoped>
.game-card {
  background: #2a2a2a;
  border-radius: 8px;
  overflow: hidden;
  cursor: pointer;
  transition: transform 0.2s, box-shadow 0.2s;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.3);
}

.game-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 4px 16px rgba(0, 0, 0, 0.5);
}

.game-cover {
  width: 100%;
  height: 200px;
  background: #333;
  display: flex;
  align-items: center;
  justify-content: center;
}

.game-cover img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.placeholder-cover {
  font-size: 4rem;
  color: #666;
  font-weight: bold;
}

.game-info {
  padding: 1rem;
}

.game-title {
  margin: 0 0 0.5rem 0;
  font-size: 1rem;
  color: white;
}

.game-core {
  margin: 0;
  font-size: 0.8rem;
  color: #aaa;
}
</style>
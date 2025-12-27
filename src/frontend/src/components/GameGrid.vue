<template>
  <div class="game-grid">
    <div class="header">
      <button @click="uiStore.backToSystems" class="back-btn">
        <svg class="icon-small" fill="currentColor" viewBox="0 0 20 20">
          <path fill-rule="evenodd" d="M12.707 5.293a1 1 0 010 1.414L9.414 10l3.293 3.293a1 1 0 01-1.414 1.414l-4-4a1 1 0 010-1.414l4-4a1 1 0 011.414 0z" clip-rule="evenodd"/>
        </svg>
        Back to Systems
      </button>
      <h2>{{ uiStore.currentSystem?.Name }} Games</h2>
    </div>
    <div v-if="filteredGames.length === 0" class="empty-state">
      <p>No games found for {{ uiStore.currentSystem?.Name }}. Add some games!</p>
    </div>
    <div v-else class="grid">
      <GameCard v-for="game in filteredGames" :key="game.Path" :game="game" />
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from "vue";
import { useGamesStore } from "../stores/games";
import { useUIStore } from "../stores/ui";
import GameCard from "./GameCard.vue";

const gamesStore = useGamesStore();
const uiStore = useUIStore();

const filteredGames = computed(() =>
  gamesStore.games.filter((g) =>
    uiStore.currentSystem ? g.SystemName === uiStore.currentSystem.Name : true
  )
);
</script>

<template>
  <div class="game-grid">
    <div class="flex items-center relative mb-8 gap-4" v-if="uiStore.currentSystem">
      <button @click="uiStore.backToSystems" class="back-btn">
        <ArrowLeft class="icon-small" />
      </button>
      <h2 class="text-white">{{ uiStore.currentSystem?.Name }}</h2>
    </div>
    <div v-if="filteredGames.length === 0" class="empty-state">
      <p>
        No games found for {{ uiStore.currentSystem?.Name }}. Add some games!
      </p>
    </div>
    <div v-else class="games-grid">
      <GameCard v-for="game in filteredGames" :key="game.Path" :game="game" />
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from "vue";
import { useGamesStore } from "../stores/games";
import { useUIStore } from "../stores/ui";
import GameCard from "./GameCard.vue";
import { ArrowLeft } from "lucide-vue-next";

const gamesStore = useGamesStore();
const uiStore = useUIStore();

const filteredGames = computed(() =>
  gamesStore.games.filter((g) =>
    uiStore.currentSystem ? g.SystemName === uiStore.currentSystem.Name : true
  )
);
</script>

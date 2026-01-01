<template>
  <div class="game-grid">
    <div class="header" v-if="uiStore.currentSystem">
      <button @click="uiStore.backToSystems" class="p-2 -ml-2 hover:bg-white/10 rounded-full transition-colors text-accent cursor-pointer" title="Back to Systems">
        <ArrowLeft :size="24" />
      </button>
      <h2 class="text-2xl font-bold">{{ uiStore.currentSystem?.Name }}</h2>
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

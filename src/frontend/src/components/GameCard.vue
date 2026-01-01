<template>
  <div class="game-card card-border group" @click="selectGame">
    <div class="game-cover">
      <img v-if="coverData" :src="coverData" alt="Cover" />
      <div v-else class="placeholder-cover">
        {{ game.Name.charAt(0).toUpperCase() }}
      </div>
      <div class="card-overlay opacity-0 group-hover:opacity-90">
        {{ game.Name }}
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from "vue";
import { useUIStore } from "../stores/ui";
import type { Game } from "../data/Game";

const props = defineProps<{
  game: Game;
}>();

const uiStore = useUIStore();
const coverData = computed(() => props.game.getCoverSrc());

const selectGame = () => {
  uiStore.selectGame(props.game);
};
</script>

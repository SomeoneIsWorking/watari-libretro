<template>
  <div class="game-card" @click="selectGame">
    <div class="game-cover">
      <img v-if="coverData" :src="coverData" alt="Cover" />
      <div v-else class="placeholder-cover">
        {{ game.Name.charAt(0).toUpperCase() }}
      </div>
      <div class="game-card-overlay">
        <h3 class="game-card-title">{{ game.Name }}</h3>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { GameInfo } from '../generated/models';
import { useUIStore } from '../stores/ui'
import { useCover } from '../composables/useCover'

const props = defineProps<{
  game: GameInfo
}>()

const uiStore = useUIStore();
const coverData = useCover(props.game.CoverName)

const selectGame = () => {
  uiStore.selectGame(props.game);
}
</script>

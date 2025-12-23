<template>
  <div class="game-display">
    <div v-if="!props.frame" class="placeholder">
      <p>Select a core and load a game to start playing</p>
    </div>
    <img v-else :src="props.frame" alt="Game Screen" class="game-screen" :class="{ 'full-screen': props.fullScreen }" />
  </div>
</template>

<script setup lang="ts">
const props = defineProps<{
  frame: string;
  fullScreen?: boolean;
}>();
</script>

<style scoped>
.game-display {
  flex: 1;
  display: flex;
  justify-content: center;
  align-items: center;
  background: #000;
  border: 2px solid #444;
  border-radius: 8px;
  margin-bottom: 1rem;
  overflow: hidden;
}

.game-display.full-screen {
  margin: 0;
  border: none;
  border-radius: 0;
}

.placeholder {
  color: #666;
  text-align: center;
  font-size: 1.2rem;
}

.game-screen {
  max-width: 100%;
  max-height: 100%;
  object-fit: contain;
}

.game-screen.full-screen {
  width: 100vw;
  height: 100vh;
  object-fit: contain;
}
</style>
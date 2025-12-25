<template>
  <div class="game-display">
    <div v-if="!props.frame" class="placeholder">
      <p>Select a core and load a game to start playing</p>
    </div>
    <canvas v-else ref="canvas" :class="{ 'full-screen': props.fullScreen }" />
  </div>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue'

const props = defineProps<{
  frame: { Pixels: string; Width: number; Height: number; PixelFormat: string } | null;
  fullScreen?: boolean;
}>();

const canvas = ref<HTMLCanvasElement | null>(null);

watch(() => props.frame, (newFrame) => {
  if (newFrame && canvas.value) {
    const ctx = canvas.value.getContext('2d');
    if (ctx) {
      canvas.value.width = newFrame.Width;
      canvas.value.height = newFrame.Height;
      const imageData = ctx.createImageData(newFrame.Width, newFrame.Height);
      const pixelData = atob(newFrame.Pixels);
      for (let i = 0; i < pixelData.length; i++) {
        imageData.data[i] = pixelData.charCodeAt(i);
      }
      ctx.putImageData(imageData, 0, 0);
    }
  }
});
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

.game-display canvas {
  max-width: 100%;
  max-height: 100%;
  object-fit: contain;
  image-rendering: pixelated;
}

.game-display canvas.full-screen {
  width: 100vw;
  height: 100vh;
}
</style>
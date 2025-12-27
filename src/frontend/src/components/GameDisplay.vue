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
import type { FrameData } from '../generated/models';

const props = defineProps<{
  frame: FrameData | null;
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
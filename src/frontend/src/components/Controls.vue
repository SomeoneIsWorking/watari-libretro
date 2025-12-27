<template>
  <div class="controls">
    <div class="rom-section">
      <h3>Load Game</h3>
      <div class="rom-input">
        <input
          :value="gameStore.romPath"
          placeholder="ROM file path"
          readonly
          class="rom-path"
        />
        <button @click="selectRomFile" class="btn btn-primary">Browse</button>
      </div>
      <div
        id="rom-drop-zone"
        class="drop-zone"
      >
        Or drop ROM file here
      </div>
      <button @click="loadGame" class="btn btn-secondary" :disabled="!gameStore.romPath">Load ROM</button>
    </div>

    <div class="run-section">
      <button @click="run" class="btn btn-success" :disabled="!coresStore.selectedCore || !gameStore.romPath || coresStore.isRunning || !gameStore.isGameLoaded">
        {{ coresStore.isRunning ? 'Running' : 'Start Game' }}
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useCoresStore } from '../stores/cores'
import { useGameStore } from '../stores/game'

const coresStore = useCoresStore()
const gameStore = useGameStore()

const selectRomFile = () => {
  gameStore.selectRomFile()
}

const loadGame = () => {
  gameStore.loadGame()
}

const run = () => {
  gameStore.run()
}
</script>
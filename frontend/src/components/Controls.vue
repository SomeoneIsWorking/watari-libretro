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

<style scoped>
.controls {
  background: #252525;
  padding: 1rem;
  border-radius: 8px;
  border: 1px solid #444;
}

.rom-section h3 {
  margin-top: 0;
  color: #00ff88;
  margin-bottom: 1rem;
}

.rom-input {
  display: flex;
  gap: 0.5rem;
  margin-bottom: 1rem;
}

.rom-path {
  flex: 1;
  padding: 0.5rem;
  background: #333;
  border: 1px solid #444;
  border-radius: 4px;
  color: white;
}

.rom-path::placeholder {
  color: #999;
}

.drop-zone {
  border: 2px dashed #444;
  padding: 1rem;
  text-align: center;
  margin-bottom: 1rem;
  border-radius: 4px;
  color: #999;
  transition: border-color 0.2s;
}

.drop-zone:hover {
  border-color: #666;
}

.run-section {
  text-align: center;
}

.btn {
  padding: 0.5rem 1rem;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 0.9rem;
  transition: background 0.2s;
}

.btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.btn-primary {
  background: #007bff;
  color: white;
}

.btn-primary:hover:not(:disabled) {
  background: #0056b3;
}

.btn-secondary {
  background: #6c757d;
  color: white;
}

.btn-secondary:hover:not(:disabled) {
  background: #545b62;
}

.btn-success {
  background: #28a745;
  color: white;
}

.btn-success:hover:not(:disabled) {
  background: #1e7e34;
}

.--watari-drop {
  border-color: #00ff88 !important;
  background-color: rgba(0, 255, 136, 0.1);
}

.--watari-drop-not-allowed {
  border-color: #ff4444 !important;
  background-color: rgba(255, 68, 68, 0.1);
}
</style>
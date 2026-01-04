<template>
  <div class="playing-view">
    <div class="game-overlay" v-if="coresStore.isMenuOpen">
      <button @click="toggleMenu" class="menu-toggle">Menu</button>
      <button @click="stopGame" class="stop-btn">Stop Game</button>
    </div>
    <GameDisplay :frame="frame" :full-screen="true" />
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useCoresStore } from '../stores/cores'
import { useUIStore } from '../stores/ui'
import { useInputStore } from '../stores/input'
import { LibretroApplication } from '../generated/libretroApplication'
import GameDisplay from './GameDisplay.vue'
import type { FrameData } from '../generated/models'

const coresStore = useCoresStore()
const uiStore = useUIStore()
const inputStore = useInputStore()
const frame = ref<FrameData | null>(null)

onMounted(() => {
  LibretroApplication.OnFrameReceived((data) => {
    frame.value = data
  })

  // Handle key presses for menu and game
  window.addEventListener('keydown', handleKeyDown)
  window.addEventListener('keyup', handleKeyUp)
})

const handleKeyDown = (e: KeyboardEvent) => {
  if (e.key === 'Escape') {
    coresStore.isMenuOpen = !coresStore.isMenuOpen
    e.preventDefault()
    return
  }
  const retroKey = inputStore.getKeyMapping(e.key)
  if (retroKey !== undefined) {
    LibretroApplication.SendKeyDown(retroKey)
    e.preventDefault()
  }
}

const handleKeyUp = (e: KeyboardEvent) => {
  const retroKey = inputStore.getKeyMapping(e.key)
  if (retroKey !== undefined) {
    LibretroApplication.SendKeyUp(retroKey)
  }
}

const toggleMenu = () => {
  coresStore.isMenuOpen = !coresStore.isMenuOpen
}

const stopGame = async () => {
  try {
    await LibretroApplication.Stop()
    coresStore.isRunning = false
    uiStore.setView('systems')
  } catch (e) {
    console.error('Error stopping game:', e)
  }
}
</script>
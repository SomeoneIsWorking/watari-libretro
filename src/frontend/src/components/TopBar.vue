<template>
  <header class="flex items-center p-4 bg-menu border-b border-menu gap-4">
    <div>
      <h1 class="text-accent text-xl font-bold">Watari</h1>
    </div>
    <div class="flex-1">
      <input
        v-model="uiStore.searchQuery"
        type="text"
        placeholder="Search games..."
        class="w-full p-2 bg-gray-700 border border-gray-600 rounded-md text-white focus:outline-none focus:border-blue-500"
      />
    </div>
    <div class="flex gap-2">
      <button @click="toggleView" class="bg-transparent border-none text-white cursor-pointer p-2 rounded hover:bg-gray-600 transition-colors">
        <Grid3X3 class="w-4 h-4" />
      </button>
      <button @click="uiStore.toggleMenu" class="bg-transparent border-none text-white cursor-pointer p-2 rounded hover:bg-gray-600 transition-colors">
        <Menu class="w-4 h-4" />
      </button>
    </div>
  </header>
</template>

<script setup lang="ts">
import { Menu, Grid3X3 } from 'lucide-vue-next'
import { useUIStore } from '../stores/ui'

const uiStore = useUIStore()

const toggleView = () => {
  if (uiStore.currentView === 'systems') {
    uiStore.selectSystem(null)
    uiStore.setView('games')
  } else {
    uiStore.setView('systems')
  }
}
</script>
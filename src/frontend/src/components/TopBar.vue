<template>
  <header class="top-bar">
    <div class="logo">
      <h1>Watari</h1>
    </div>
    <div class="search">
      <input
        v-model="uiStore.searchQuery"
        type="text"
        placeholder="Search games..."
        class="input"
      />
    </div>
    <div class="actions">
      <button @click="uiStore.toggleMenu" class="btn btn-secondary">
        Menu
      </button>
    </div>
  </header>
</template>

<script setup lang="ts">
import { useUIStore } from '../stores/ui'

const uiStore = useUIStore()
</script>
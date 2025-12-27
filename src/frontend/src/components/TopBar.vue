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

<style scoped>
.top-bar {
  display: flex;
  align-items: center;
  padding: 1rem;
  background: #1a1a1a;
  border-bottom: 1px solid #333;
  gap: 1rem;
}

.logo h1 {
  color: #00d4aa;
  font-size: 1.5rem;
  font-weight: bold;
}

.search {
  flex: 1;
  max-width: 400px;
}

.input {
  width: 100%;
}

.actions {
  display: flex;
  gap: 0.5rem;
}
</style>
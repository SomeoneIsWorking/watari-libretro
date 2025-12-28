<template>
  <div class="p-8">
    <div v-if="systemsWithGames.length === 0" class="empty-state">
      <p>No systems with games. Add some games to get started!</p>
    </div>
    <div v-else class="systems-grid">
      <SystemCard
        v-for="system in systemsWithGames"
        :key="system.Name"
        :system="system"
      />
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useGamesStore } from '../stores/games'
import SystemCard from './SystemCard.vue'

const gamesStore = useGamesStore()

const systemsWithGames = computed(() => gamesStore.getSystemsWithGames)
</script>
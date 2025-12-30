<template>
  <div class="p-8">
    <h1 class="text-2xl font-bold mb-4">Cores</h1>
    <div class="cores-grid">
      <CoreCard
        v-for="core in coresStore.cores"
        :key="core.id"
        :core="core"
      />
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted } from 'vue'
import { useCoresStore } from '../stores/cores'
import CoreCard from './CoreCard.vue'

const coresStore = useCoresStore()

onMounted(async () => {
  await coresStore.initialize()
})
</script>
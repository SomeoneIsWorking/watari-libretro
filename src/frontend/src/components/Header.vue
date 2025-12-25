<template>
  <header class="header">
    <h1>Watari Libretro Frontend</h1>
    <div class="status">{{ props.status }}</div>
  </header>
</template>

<script setup lang="ts">
const props = defineProps<{
  status: string;
}>();
</script>

<style scoped>
.header {
  background: #2d2d2d;
  padding: 1rem;
  border-bottom: 2px solid #444;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.header h1 {
  margin: 0;
  color: #00ff88;
  font-size: 1.5rem;
}

.status {
  font-size: 0.9rem;
  color: #ccc;
}
</style>
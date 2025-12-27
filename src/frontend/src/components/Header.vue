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
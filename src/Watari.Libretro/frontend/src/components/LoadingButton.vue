<template>
  <button :disabled="loading" :class="`btn flex items-center btn-${variant || 'primary'}`" @click="$emit('click')">
    <LoaderCircle v-if="loading" class="animate-spin mr-2" :size="16" />
    {{ loading ? loadingText : text }}
  </button>
</template>

<script setup lang="ts">
import { LoaderCircle } from 'lucide-vue-next'

const props = defineProps<{
  loading: boolean
  text: string
  loadingText: string
  variant?: 'primary' | 'secondary' | 'success'
}>()

defineEmits<{
  click: []
}>()
</script>
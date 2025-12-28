<template>
  <div v-if="toasts.length" class="toast-container">
    <div
      v-for="(toast, index) in toasts"
      :key="index"
      class="toast"
      :class="toast.type"
    >
      {{ toast.message }}
      <button @click="removeToast(index)" class="close-btn">
        <X class="icon-small" />
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useToast } from '../composables/useToast';
import { X } from 'lucide-vue-next'

const { toasts, removeToast } = useToast();
</script>
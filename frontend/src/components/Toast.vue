<template>
  <div v-if="toasts.length" class="toast-container">
    <div
      v-for="(toast, index) in toasts"
      :key="index"
      class="toast"
      :class="toast.type"
    >
      {{ toast.message }}
      <button @click="removeToast(index)" class="close-btn">&times;</button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useToast } from '../composables/useToast';

const { toasts, removeToast } = useToast();
</script>

<style scoped>
.toast-container {
  position: fixed;
  top: 20px;
  right: 20px;
  z-index: 1000;
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.toast {
  background: #333;
  color: white;
  padding: 10px 15px;
  border-radius: 4px;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.5);
  display: flex;
  justify-content: space-between;
  align-items: center;
  min-width: 250px;
}

.toast.success {
  border-left: 4px solid #28a745;
}

.toast.error {
  border-left: 4px solid #dc3545;
}

.toast.info {
  border-left: 4px solid #007bff;
}

.close-btn {
  background: none;
  border: none;
  color: white;
  font-size: 20px;
  cursor: pointer;
  margin-left: 10px;
}
</style>
<template>
  <aside class="sidebar" :class="{ overlay: props.overlay }">
    <h2>Cores</h2>
    <div class="core-list">
      <div
        v-for="core in store.getCoreList()"
        :key="core.name"
        class="core-item"
        :class="{ selected: store.selectedCore === core.name }"
      >
        <div class="core-info">
          <span class="core-name">{{ core.name }}</span>
          <span v-if="core.status.value === 'downloaded'" class="status-icon">✓</span>
          <span v-else-if="core.status.value === 'downloading'" class="status-icon">⬇</span>
        </div>
        <div class="core-actions">
          <button
            v-if="core.status.value === 'available'"
            @click="handleDownloadCore(core.name)"
            class="btn btn-primary"
          >
            Download
          </button>
          <div v-else-if="core.status.value === 'downloading'" class="progress-container">
            <progress :value="core.progress.value" max="100"></progress>
            <span>{{ Math.round(core.progress.value) }}%</span>
          </div>
          <button
            v-if="core.status.value === 'downloaded'"
            @click="handleLoadCore(core.name)"
            class="btn btn-secondary"
          >
            Load
          </button>
        </div>
      </div>
    </div>
  </aside>
</template>

<script setup lang="ts">
import { useCoresStore } from '../stores/cores';
import { useToast } from '../composables/useToast';

const store = useCoresStore();
const { addToast } = useToast();

const props = defineProps<{
  overlay?: boolean;
}>();

const handleDownloadCore = async (name: string) => {
  try {
    await store.downloadCore(name);
    addToast(`Downloaded core: ${name}`, 'success');
  } catch (e) {
    addToast('Error downloading core: ' + e, 'error');
  }
};

const handleLoadCore = async (name: string) => {
  try {
    await store.loadCore(name);
    addToast(`Loaded core: ${name}`, 'success');
  } catch (e) {
    addToast('Error loading core: ' + e, 'error');
  }
};
</script>

<style scoped>
.sidebar {
  width: 300px;
  background: #252525;
  padding: 1rem;
  border-right: 1px solid #444;
  overflow-y: auto;
}

.sidebar.overlay {
  position: fixed;
  top: 0;
  right: 0;
  height: 100vh;
  z-index: 1000;
  background: rgba(37, 37, 37, 0.9);
}

.sidebar h2 {
  margin-top: 0;
  color: #00ff88;
  border-bottom: 1px solid #444;
  padding-bottom: 0.5rem;
}

.core-list {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.core-item {
  background: #333;
  padding: 0.75rem;
  border-radius: 4px;
  border: 1px solid #444;
  transition: background 0.2s;
}

.core-item:hover {
  background: #3a3a3a;
}

.core-item.selected {
  border-color: #00ff88;
  background: #2a4a2a;
}

.core-info {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 0.5rem;
}

.core-name {
  font-weight: bold;
}

.status-icon {
  color: #00ff88;
  font-size: 1.2rem;
}

.core-actions {
  display: flex;
  gap: 0.5rem;
  align-items: center;
}

.btn {
  padding: 0.5rem 1rem;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 0.9rem;
  transition: background 0.2s;
}

.btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.btn-primary {
  background: #007bff;
  color: white;
}

.btn-primary:hover:not(:disabled) {
  background: #0056b3;
}

.btn-secondary {
  background: #6c757d;
  color: white;
}

.btn-secondary:hover:not(:disabled) {
  background: #545b62;
}

.progress-container {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  width: 100%;
}

progress {
  flex: 1;
  height: 8px;
}
</style>
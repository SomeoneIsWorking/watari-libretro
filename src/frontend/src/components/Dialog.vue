<template>
  <Teleport to="body">
    <Transition name="dialog">
      <div v-if="modelValue" class="dialog-overlay" @click.self="modelValue = false">
        <div class="dialog-content">
          <div class="dialog-header">
            <h3 class="dialog-title">{{ title }}</h3>
            <button @click="modelValue = false" class="dialog-close">
              <X class="icon-small" />
            </button>
          </div>
          <div class="dialog-body">
            <slot></slot>
          </div>
        </div>
      </div>
    </Transition>
  </Teleport>
</template>

<script setup lang="ts">
import { X } from 'lucide-vue-next'

const props = defineProps<{
  title: string
}>()

const modelValue = defineModel<boolean>()
</script>
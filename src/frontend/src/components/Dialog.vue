<template>
  <Teleport to="body">
    <Transition name="dialog">
      <div v-if="modelValue" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-[1000]" @click.self="modelValue = false">
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-lg min-w-[400px] max-w-[95vw] max-h-[90vh] flex flex-col" v-bind="$attrs">
          <div class="flex justify-between items-center p-4 border-b border-gray-200 dark:border-gray-700">
            <h3 class="m-0 text-xl font-semibold text-gray-900 dark:text-white">{{ title }}</h3>
            <button @click="modelValue = false" class="bg-none border-none cursor-pointer p-1 rounded hover:bg-gray-100 dark:hover:bg-gray-700 flex items-center justify-center">
              <X class="w-4 h-4 text-gray-500 dark:text-gray-400" />
            </button>
          </div>
          <div :class="['p-4 flex-1 text-gray-700 dark:text-gray-300', contentClass]">
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
  contentClass?: string
}>()

const modelValue = defineModel<boolean>()

defineOptions({
  inheritAttrs: false
})
</script>
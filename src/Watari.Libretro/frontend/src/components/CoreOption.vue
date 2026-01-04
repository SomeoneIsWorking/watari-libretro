<template>
  <div class="option-item flex items-center gap-4">
    <label class="text-sm text-right font-medium flex-shrink-0 w-48">
      {{ description }}:
    </label>
    <div class="flex-1 flex items-center gap-2">
      <!-- Select dropdown -->
      <select
        v-if="optionType === 'select'"
        :value="currentValue"
        @change="handleChange"
        class="flex-1"
      >
        <option v-for="option in options" :key="option" :value="option">
          {{ option }}{{ option === defaultValue ? " (default)" : "" }}
        </option>
      </select>

      <!-- Numeric slider -->
      <div
        v-else-if="optionType === 'numeric'"
        class="flex-1 flex items-center gap-2"
      >
        <input
          type="range"
          :min="numericMin"
          :max="numericMax"
          :step="numericStep"
          :value="currentValue"
          @input="handleChange"
          class="flex-1"
        />
        <span class="text-xs text-gray-400">{{ currentValue }}</span>
        <span class="text-xs text-gray-500">(default: {{ defaultValue }})</span>
      </div>

      <!-- Boolean toggle -->
      <div v-else-if="optionType === 'boolean'" class="flex items-center gap-2">
        <label class="relative inline-flex items-center cursor-pointer">
          <input
            type="checkbox"
            :checked="currentValue === 'true' || currentValue === 'enabled'"
            @change="handleToggle"
            class="sr-only peer"
          />
          <div
            class="w-11 h-6 bg-gray-600 peer-focus:outline-none peer-focus:ring-4 peer-focus:ring-blue-300 rounded-full peer peer-checked:after:translate-x-full peer-checked:after:border-white after:content-[''] after:absolute after:top-[2px] after:left-[2px] after:bg-white after:rounded-full after:h-5 after:w-5 after:transition-all peer-checked:bg-blue-600"
          ></div>
        </label>
        <span class="text-xs text-gray-500">(default: {{ defaultValue }})</span>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from "vue";

const props = defineProps<{
  optionKey: string;
  definition: string;
  currentValue: string;
  onChange: (key: string, value: string) => void;
}>();

const emit = defineEmits<{
  change: [key: string, value: string];
}>();

const description = computed(() => {
  const parts = props.definition.split(";");
  return parts[0]?.trim() || props.definition;
});

const options = computed(() => {
  const parts = props.definition.split(";");
  if (parts.length > 1) {
    return parts[1]!.split("|").map((o) => o.trim());
  }
  return [];
});

const defaultValue = computed(() => {
  const parts = props.definition.split(";");
  if (parts.length > 2) {
    return parts[2]!.trim();
  }
  return options.value[0] || "";
});

const optionType = computed(() => {
  const opts = options.value;
  if (opts.length === 0) return "select";

  // Check if all options are numbers
  if (opts.every((opt) => !isNaN(Number(opt)))) {
    return "numeric";
  }

  // Check if boolean (true/false or enabled/disabled)
  const lowerOpts = opts.map((o) => o.toLowerCase());
  if (
    (lowerOpts.includes("true") && lowerOpts.includes("false")) ||
    (lowerOpts.includes("enabled") && lowerOpts.includes("disabled"))
  ) {
    return "boolean";
  }

  return "select";
});

const numericMin = computed(() => {
  const opts = options.value.map(Number).filter((n) => !isNaN(n));
  return Math.min(...opts);
});

const numericMax = computed(() => {
  const opts = options.value.map(Number).filter((n) => !isNaN(n));
  return Math.max(...opts);
});

const numericStep = computed(() => {
  // Assume step 1 for now, could be smarter
  return 1;
});

const handleChange = (event: Event) => {
  const target = event.target as HTMLInputElement | HTMLSelectElement;
  emit("change", props.optionKey, target.value);
};

const handleToggle = (event: Event) => {
  const target = event.target as HTMLInputElement;
  const value = target.checked
    ? options.value.includes("true")
      ? "true"
      : "enabled"
    : options.value.includes("false")
    ? "false"
    : "disabled";
  emit("change", props.optionKey, value);
};
</script>

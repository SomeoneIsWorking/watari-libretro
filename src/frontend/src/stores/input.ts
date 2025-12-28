import { ref } from 'vue'
import { defineStore } from 'pinia'

export const useInputStore = defineStore('input', () => {
  const keyMappings = ref<Record<string, string>>({
    ArrowUp: 'RETRO_DEVICE_ID_JOYPAD_UP',
    ArrowDown: 'RETRO_DEVICE_ID_JOYPAD_DOWN',
    ArrowLeft: 'RETRO_DEVICE_ID_JOYPAD_LEFT',
    ArrowRight: 'RETRO_DEVICE_ID_JOYPAD_RIGHT',
    z: 'RETRO_DEVICE_ID_JOYPAD_A',
    x: 'RETRO_DEVICE_ID_JOYPAD_B',
    a: 'RETRO_DEVICE_ID_JOYPAD_X',
    s: 'RETRO_DEVICE_ID_JOYPAD_Y',
    d: 'RETRO_DEVICE_ID_JOYPAD_L',
    c: 'RETRO_DEVICE_ID_JOYPAD_R',
    Enter: 'RETRO_DEVICE_ID_JOYPAD_START',
    Shift: 'RETRO_DEVICE_ID_JOYPAD_SELECT',
  })

  const getKeyMapping = (key: string) => keyMappings.value[key]

  return {
    keyMappings,
    getKeyMapping
  }
})
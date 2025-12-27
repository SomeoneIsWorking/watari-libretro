<template>
  <transition name="backdrop">
    <div v-if="uiStore.isMenuOpen" class="backdrop" @click="uiStore.toggleMenu"></div>
  </transition>
  <aside class="side-menu" :class="{ open: uiStore.isMenuOpen }">
    <div class="menu-header">
      <h2>Menu</h2>
      <button @click="uiStore.toggleMenu" class="close-btn">
        <svg class="icon-small" fill="currentColor" viewBox="0 0 20 20">
          <path fill-rule="evenodd" d="M4.293 4.293a1 1 0 011.414 0L10 8.586l4.293-4.293a1 1 0 111.414 1.414L11.414 10l4.293 4.293a1 1 0 01-1.414 1.414L10 11.414l-4.293 4.293a1 1 0 01-1.414-1.414L8.586 10 4.293 5.707a1 1 0 010-1.414z" clip-rule="evenodd"/>
        </svg>
      </button>
    </div>
    <nav class="menu-nav">
      <ul>
        <li>
          <button @click="navigateTo('systems')" class="menu-item">
            <svg class="icon" fill="currentColor" viewBox="0 0 20 20">
              <path d="M3 4a1 1 0 011-1h12a1 1 0 011 1v2a1 1 0 01-1 1H4a1 1 0 01-1-1V4zM3 10a1 1 0 011-1h6a1 1 0 011 1v6a1 1 0 01-1 1H4a1 1 0 01-1-1v-6zM14 9a1 1 0 00-1 1v6a1 1 0 001 1h2a1 1 0 001-1v-6a1 1 0 00-1-1h-2z"/>
            </svg>
            Systems
          </button>
        </li>
        <li>
          <button @click="navigateTo('all-games')" class="menu-item">
            <svg class="icon" fill="currentColor" viewBox="0 0 20 20">
              <path d="M7 3a1 1 0 000 2h6a1 1 0 100-2H7zM4 7a1 1 0 011-1h10a1 1 0 011 1v1a1 1 0 01-1 1H5a1 1 0 01-1-1V7zM3 11a1 1 0 011-1h12a1 1 0 011 1v1a1 1 0 01-1 1H4a1 1 0 01-1-1v-1z"/>
            </svg>
            All Games
          </button>
        </li>
        <li>
          <button @click="addGame" class="menu-item">
            <svg class="icon" fill="currentColor" viewBox="0 0 20 20">
              <path fill-rule="evenodd" d="M10 3a1 1 0 011 1v5h5a1 1 0 110 2h-5v5a1 1 0 11-2 0v-5H4a1 1 0 110-2h5V4a1 1 0 011-1z" clip-rule="evenodd"/>
            </svg>
            Add Game
          </button>
        </li>
        <li>
          <button @click="navigateTo('settings')" class="menu-item">
            <svg class="icon" fill="currentColor" viewBox="0 0 20 20">
              <path fill-rule="evenodd" d="M11.49 3.17c-.38-1.56-2.6-1.56-2.98 0a1.532 1.532 0 01-2.286.948c-1.372-.836-2.942.734-2.106 2.106.54.886.061 2.042-.947 2.287-1.561.379-1.561 2.6 0 2.978a1.532 1.532 0 01.947 2.287c-.836 1.372.734 2.942 2.106 2.106a1.532 1.532 0 012.287.947c.379 1.561 2.6 1.561 2.978 0a1.533 1.533 0 012.287-.947c1.372.836 2.942-.734 2.106-2.106a1.533 1.533 0 01.947-2.287c1.561-.379 1.561-2.6 0-2.978a1.532 1.532 0 01-.947-2.287c.836-1.372-.734-2.942-2.106-2.106a1.532 1.532 0 01-2.287-.947zM10 13a3 3 0 100-6 3 3 0 000 6z" clip-rule="evenodd"/>
            </svg>
            Settings
          </button>
        </li>
      </ul>
    </nav>
  </aside>

  <Dialog v-model="showModal" title="Select System">
    <ul>
      <li v-for="sys in applicableSystems" :key="sys.Name">
        <button @click="selectSystem(sys.Name)" class="system-option">{{ sys.Name }}</button>
      </li>
    </ul>
  </Dialog>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useUIStore } from '../stores/ui'
import { useGamesStore } from '../stores/games'
import { useToast } from '../composables/useToast'
import { LibretroApplication } from '../generated/libretroApplication'
import type { SystemInfo } from '../generated/models'
import Dialog from './Dialog.vue'

const uiStore = useUIStore()
const gamesStore = useGamesStore()
const { addToast } = useToast()

const applicableSystems = ref<SystemInfo[]>([])
let currentPath = ''
const showModal = ref(false)

const navigateTo = (view: string) => {
  if (view === 'systems') {
    uiStore.setView('systems')
    uiStore.currentSystem = null
  } else if (view === 'all-games') {
    uiStore.setView('games')
    uiStore.currentSystem = null
  } else if (view === 'settings') {
    // For now, just toast
    addToast('Settings not implemented yet', 'info')
  }
  uiStore.toggleMenu()
}

const addGame = async () => {
  try {
    const path = await watari.openFileDialog('nes,snes,gb,gba,md,gen,z64,n64,cue,bin,iso')
    if (path) {
      await gamesStore.loadSystems()
      currentPath = path
      const ext = path.split('.').pop()?.toLowerCase() || ''
      applicableSystems.value = gamesStore.systems.filter(s => s.Extensions.some(e => e.toLowerCase() === ext))
      if (applicableSystems.value.length === 1) {
        await proceedWithSystem(applicableSystems.value[0]!.Name)
      } else if (applicableSystems.value.length > 1) {
        showModal.value = true
      } else {
        addToast('No system found for this file type', 'error')
      }
    }
  } catch (e) {
    addToast('Error adding game: ' + e, 'error')
  }
  uiStore.toggleMenu()
}

const selectSystem = async (systemName: string) => {
  showModal.value = false
  await proceedWithSystem(systemName)
}

const proceedWithSystem = async (systemName: string) => {
  const name = await getGameName(currentPath)
  const game = {
    Name: name,
    Path: currentPath,
    SystemName: systemName,
    CoverName: ""
  }
  await LibretroApplication.AddGame(game)
  await gamesStore.loadLibrary()
  addToast('Added game', 'success')
}

const getGameName = async (path: string) => {
  return path.split('/').pop()?.split('.')[0] || 'Unknown'
}
</script>
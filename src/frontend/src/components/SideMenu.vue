<template>
  <aside class="side-menu" :class="{ open: uiStore.isMenuOpen }">
    <div class="menu-header">
      <h2>Menu</h2>
      <button @click="uiStore.toggleMenu" class="close-btn">×</button>
    </div>
    <nav class="menu-nav">
      <ul>
        <li>
          <button @click="navigateTo('systems')" class="menu-item">
            Systems
          </button>
        </li>
        <li>
          <button @click="navigateTo('all-games')" class="menu-item">
            All Games
          </button>
        </li>
        <li>
          <button @click="addGame" class="menu-item">
            Add Game
          </button>
        </li>
        <li>
          <button @click="navigateTo('settings')" class="menu-item">
            Settings
          </button>
        </li>
      </ul>
    </nav>
  </aside>

  <dialog ref="systemModal" class="system-modal">
    <h3>Select System</h3>
    <ul>
      <li v-for="sys in applicableSystems" :key="sys.Name">
        <button @click="selectSystem(sys.Name)" class="system-option">{{ sys.Name }}</button>
      </li>
    </ul>
    <button @click="closeModal" class="cancel-btn">Cancel</button>
  </dialog>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useUIStore } from '../stores/ui'
import { useGamesStore } from '../stores/games'
import { useToast } from '../composables/useToast'
import { LibretroApplication } from '../generated/libretroApplication'
import type { SystemInfo } from '../generated/models'

const uiStore = useUIStore()
const gamesStore = useGamesStore()
const { addToast } = useToast()

const systemModal = ref<HTMLDialogElement>()
const applicableSystems = ref<SystemInfo[]>([])
let currentPath = ''

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
        systemModal.value?.showModal()
      } else {
        addToast('No system found for this file type', 'error')
      }
    }
  } catch (e) {
    addToast('Error adding game: ' + e, 'error')
  }
  uiStore.toggleMenu()
}

const selectSystem = async (systemId: string) => {
  systemModal.value?.close()
  await proceedWithSystem(systemId)
}

const closeModal = () => {
  systemModal.value?.close()
}

const proceedWithSystem = async (systemId: string) => {
  const name = await getGameName(currentPath)
  const game = {
    Name: name,
    Path: currentPath,
    SystemName: systemId,
    Cover: ""
  }
  await LibretroApplication.AddGame(game)
  await gamesStore.loadLibrary()
  addToast('Added game', 'success')
}

const getGameName = async (path: string) => {
  return path.split('/').pop()?.split('.')[0] || 'Unknown'
}
</script>

<style scoped>
.side-menu {
  position: fixed;
  top: 0;
  left: -300px;
  width: 300px;
  height: 100vh;
  background: #1a1a1a;
  border-right: 1px solid #333;
  transition: left 0.3s;
  z-index: 1000;
  display: flex;
  flex-direction: column;
}

.side-menu.open {
  left: 0;
}

.menu-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1rem;
  border-bottom: 1px solid #333;
}

.menu-header h2 {
  color: #00d4aa;
  margin: 0;
}

.close-btn {
  background: none;
  border: none;
  color: white;
  font-size: 1.5rem;
  cursor: pointer;
}

.menu-nav ul {
  list-style: none;
  padding: 0;
  margin: 0;
}

.menu-item {
  width: 100%;
  padding: 1rem;
  background: none;
  border: none;
  color: white;
  text-align: left;
  cursor: pointer;
  transition: background 0.2s;
}

.menu-item:hover {
  background: #2a2a2a;
}

.system-modal {
  background: #1a1a1a;
  color: white;
  border: 1px solid #333;
  border-radius: 8px;
  padding: 1rem;
}

.system-modal h3 {
  margin-top: 0;
  color: #00d4aa;
}

.system-modal ul {
  list-style: none;
  padding: 0;
}

.system-option {
  width: 100%;
  padding: 0.5rem;
  background: #2a2a2a;
  border: none;
  color: white;
  cursor: pointer;
  margin-bottom: 0.5rem;
}

.system-option:hover {
  background: #3a3a3a;
}

.cancel-btn {
  background: #333;
  border: none;
  color: white;
  padding: 0.5rem 1rem;
  cursor: pointer;
}

.cancel-btn:hover {
  background: #444;
}
</style>
<template>
  <transition name="backdrop">
    <div
      v-if="uiStore.isMenuOpen"
      class="backdrop"
      @click="uiStore.toggleMenu"
    ></div>
  </transition>
  <aside class="side-menu" :class="{ open: uiStore.isMenuOpen }">
    <div class="menu-header">
      <h2>Menu</h2>
      <button @click="uiStore.toggleMenu" class="close-btn">
        <X class="icon-small" />
      </button>
    </div>
    <nav class="menu-nav">
      <ul>
        <li>
          <button @click="navigateTo('systems')" class="menu-item">
            <LayoutGrid class="icon" />
            Systems
          </button>
        </li>
        <li>
          <button @click="navigateTo('all-games')" class="menu-item">
            <List class="icon" />
            All Games
          </button>
        </li>
        <li>
          <button @click="addGame" class="menu-item">
            <Plus class="icon" />
            Add Game
          </button>
        </li>
        <li>
          <button @click="navigateTo('settings')" class="menu-item">
            <Settings class="icon" />
            Settings
          </button>
        </li>
      </ul>
    </nav>
  </aside>

  <Dialog v-model="showModal" title="Select System">
    <ul>
      <li v-for="sys in applicableSystems" :key="sys.Name">
        <button @click="selectSystem(sys.Name)" class="system-option">
          {{ sys.Name }}
        </button>
      </li>
    </ul>
  </Dialog>

  <SettingsModal v-model="showSettingsModal" />
</template>

<script setup lang="ts">
import { ref } from "vue";
import { useUIStore } from "../stores/ui";
import { useGamesStore } from "../stores/games";
import { useToast } from "../composables/useToast";
import { LibretroApplication } from "../generated/libretroApplication";
import type { SystemInfo } from "../generated/models";
import Dialog from "./Dialog.vue";
import SettingsModal from "./SettingsModal.vue";
import { X, LayoutGrid, List, Plus, Settings } from "lucide-vue-next";

const uiStore = useUIStore();
const gamesStore = useGamesStore();
const { addToast } = useToast();

const applicableSystems = ref<SystemInfo[]>([]);
let currentPath = "";
const showModal = ref(false);
const showSettingsModal = ref(false);

const navigateTo = (view: string) => {
  if (view === "systems") {
    uiStore.setView("systems");
    uiStore.currentSystem = null;
  } else if (view === "all-games") {
    uiStore.setView("games");
    uiStore.currentSystem = null;
  } else if (view === "settings") {
    showSettingsModal.value = true;
  }
  uiStore.toggleMenu();
};

const addGame = async () => {
  try {
    const path = await watari.openFileDialog(
      "nes,snes,gb,gba,md,gen,z64,n64,cue,bin,iso"
    );
    if (!path) {
      throw new Error("No file selected");
    }
    await gamesStore.loadSystems();
    currentPath = path;
    const ext = path.split(".").pop()?.toLowerCase() || "";
    applicableSystems.value = gamesStore.systems.filter((s) =>
      s.Extensions.some((e) => e.toLowerCase() === ext)
    );
    console.log(
      `Applicable systems for extension .${ext}: ${applicableSystems.value
        .map((s) => s.Name)
        .join(", ")}`
    );
    if (applicableSystems.value.length === 1) {
      console.log(
        `Single applicable system found: ${applicableSystems.value[0]!.Name}`
      );
      await proceedWithSystem(applicableSystems.value[0]!.Name);
    } else if (applicableSystems.value.length > 1) {
      showModal.value = true;
    } else {
      addToast("No system found for this file type", "error");
    }
  } catch (e) {
    addToast("Error adding game: " + e, "error");
  }
  uiStore.toggleMenu();
};

const selectSystem = async (systemName: string) => {
  showModal.value = false;
  await proceedWithSystem(systemName);
};

const proceedWithSystem = async (systemName: string) => {
  const name = getGameName(currentPath);
  await LibretroApplication.AddGame({
    CoverName: "",
    Name: name,
    Path: currentPath,
    SystemName: systemName,
  });
  await gamesStore.loadLibrary();
  addToast("Added game", "success");
};

const getGameName = (path: string) => {
  return path.split("/").pop()?.split(".")[0] || "Unknown";
};
</script>

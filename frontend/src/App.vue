<script setup lang="ts">
import { ref, onMounted } from "vue";
import { LibretroApplication } from "./generated/libretroApplication";
import { useToast } from "./composables/useToast";
import { useCoresStore } from "./stores/cores";
import { useGameStore } from "./stores/game";
import Header from "./components/Header.vue";
import Sidebar from "./components/Sidebar.vue";
import GameDisplay from "./components/GameDisplay.vue";
import Controls from "./components/Controls.vue";
import Toast from "./components/Toast.vue";

const { addToast } = useToast();
const store = useCoresStore();
const gameStore = useGameStore();

const frame = ref<string>("");

onMounted(async () => {
  try {
    await store.initialize();
  } catch (e) {
    addToast("Error loading cores: " + e, "error");
  }

  LibretroApplication.OnFrameReceived((data) => {
    frame.value = data.Image;
  });

  LibretroApplication.OnDownloadProgress((data) => {
    const core = store.cores.find((c) => c.name === data.Name);
    if (core) {
      core.setProgress(data.Progress);
    }
  });

  watari.drop_zone("rom-drop-zone", (paths: string[]) => {
    if (paths.length > 0) {
      gameStore.romPath = paths[0]!;
      gameStore.status = `ROM selected: ${paths[0]}`;
      addToast(`ROM selected: ${paths[0]}`, "info");
    }
  });

  window.addEventListener("keydown", handleKeyDown);
  window.addEventListener("keyup", handleKeyUp);
});

const handleKeyDown = (e: KeyboardEvent) => {
  if (!store.isRunning) return;
  if (e.key === "m" || e.key === "M") {
    store.isMenuOpen = !store.isMenuOpen;
    e.preventDefault();
    return;
  }
  const keyMap: Record<string, string> = {
    arrowup: "RETRO_DEVICE_ID_JOYPAD_UP",
    arrowdown: "RETRO_DEVICE_ID_JOYPAD_DOWN",
    arrowleft: "RETRO_DEVICE_ID_JOYPAD_LEFT",
    arrowright: "RETRO_DEVICE_ID_JOYPAD_RIGHT",
    z: "RETRO_DEVICE_ID_JOYPAD_A",
    x: "RETRO_DEVICE_ID_JOYPAD_B",
    a: "RETRO_DEVICE_ID_JOYPAD_X",
    s: "RETRO_DEVICE_ID_JOYPAD_Y",
    d: "RETRO_DEVICE_ID_JOYPAD_L",
    c: "RETRO_DEVICE_ID_JOYPAD_R",
  };
  const retroKey = keyMap[e.key.toLowerCase()];
  if (retroKey !== undefined) {
    LibretroApplication.SendKeyDown(retroKey);
    e.preventDefault();
  }
};

const handleKeyUp = (e: KeyboardEvent) => {
  if (!store.isRunning) {
    return;
  }
  e.preventDefault();
  const keyMap: Record<string, string> = {
    arrowup: "RETRO_DEVICE_ID_JOYPAD_UP",
    arrowdown: "RETRO_DEVICE_ID_JOYPAD_DOWN",
    arrowleft: "RETRO_DEVICE_ID_JOYPAD_LEFT",
    arrowright: "RETRO_DEVICE_ID_JOYPAD_RIGHT",
    z: "RETRO_DEVICE_ID_JOYPAD_A",
    x: "RETRO_DEVICE_ID_JOYPAD_B",
    a: "RETRO_DEVICE_ID_JOYPAD_X",
    s: "RETRO_DEVICE_ID_JOYPAD_Y",
    q: "RETRO_DEVICE_ID_JOYPAD_L",
    w: "RETRO_DEVICE_ID_JOYPAD_R",
  };
  const retroKey = keyMap[e.key.toLowerCase()];
  if (retroKey !== undefined) {
    LibretroApplication.SendKeyUp(retroKey);
  }
};
</script>

<template>
  <div class="app">
    <Header v-if="!store.isRunning" :status="gameStore.status" />
    <div class="main-content" :class="{ 'full-screen': store.isRunning }">
      <Sidebar
        :overlay="store.isRunning"
        v-show="!store.isRunning || store.isMenuOpen"
      />
      <main class="game-area" :class="{ 'full-screen': store.isRunning }">
        <GameDisplay :frame="frame" :full-screen="store.isRunning" />
        <Controls v-if="!store.isRunning" />
      </main>
    </div>
    <Toast />
  </div>
</template>

<style scoped>
.app {
  display: flex;
  flex-direction: column;
  height: 100vh;
  background: #1a1a1a;
  color: #ffffff;
  font-family: "Segoe UI", Tahoma, Geneva, Verdana, sans-serif;
}

.main-content {
  display: flex;
  flex: 1;
  overflow: hidden;
}

.main-content.full-screen {
  position: fixed;
  top: 0;
  left: 0;
  width: 100vw;
  height: 100vh;
  z-index: 10;
}

.game-area {
  flex: 1;
  display: flex;
  flex-direction: column;
  padding: 1rem;
  overflow: hidden;
}

.game-area.full-screen {
  padding: 0;
}
</style>

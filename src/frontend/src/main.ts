import { createApp } from 'vue'
import { createPinia } from 'pinia'
import './styles/main.css'
import App from './App.vue'
import { globalAddToast } from './composables/useToast'
import { useCoresStore } from './stores/cores'
import { useSettingsStore } from './stores/settings'
import { useGamesStore } from './stores/games'

const app = createApp(App)
const pinia = createPinia()

app.use(pinia)

// Global error handler
app.config.errorHandler = (err) => {
  globalAddToast(`An error occurred: ${err}`, 'error')
  console.error(err)
}

// Global unhandled promise rejection handler
window.addEventListener('unhandledrejection', (event) => {
  globalAddToast(`Unhandled promise rejection: ${event.reason}`, 'error')
  console.error(event.reason)
  event.preventDefault() // Prevent the default browser behavior
})
async function initializeApp() {
  await Promise.all([
    useCoresStore().initialize(),
    useGamesStore().loadLibrary(),
    useSettingsStore().loadSettings()
  ])
  app.mount('#app')
}

initializeApp().catch((e) => {
  globalAddToast(`Failed to initialize application: ${e}`, 'error')
  console.error('Failed to initialize application:', e)
})
import { createApp } from 'vue'
import { createPinia } from 'pinia'
import './styles/main.css'
import App from './App.vue'
import { globalAddToast } from './composables/useToast'
import { useCoresStore } from './stores/cores'
import { useSettingsStore } from './stores/settings'

const app = createApp(App)
const pinia = createPinia()

app.use(pinia)

// Global error handler
app.config.errorHandler = (err) => {
  globalAddToast(`An error occurred: ${err}`, 'error')
  console.error(err)
}
async function initializeApp() {
  await Promise.all([
    useCoresStore().initialize(),
    useSettingsStore().loadSettings()
  ])
  app.mount('#app')
}

initializeApp().catch((e) => {
  globalAddToast(`Failed to initialize application: ${e}`, 'error')
  console.error('Failed to initialize application:', e)
})
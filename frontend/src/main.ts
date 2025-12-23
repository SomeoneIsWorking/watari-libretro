import { createApp } from 'vue'
import { createPinia } from 'pinia'
import './style.css'
import App from './App.vue'
import { globalAddToast } from './composables/useToast'
import { useCoresStore } from './stores/cores'

const app = createApp(App)
const pinia = createPinia()

app.use(pinia)

// Global error handler
app.config.errorHandler = (err) => {
  globalAddToast(`An error occurred: ${err}`, 'error')
  console.error(err)
}
async function initializeApp() {
  await useCoresStore().initialize()
  app.mount('#app')
}

initializeApp().catch((e) => {
  globalAddToast(`Failed to initialize application: ${e}`, 'error')
  console.error('Failed to initialize application:', e)
})
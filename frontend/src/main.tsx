import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import './index.css'
import App from './App.tsx'
import { LocalizationProvider } from '@mui/x-date-pickers/LocalizationProvider'
import { AdapterDateFns } from '@mui/x-date-pickers/AdapterDateFns'
import { ar } from 'date-fns/locale'

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <LocalizationProvider dateAdapter={AdapterDateFns} adapterLocale={ar}>
      <App />
    </LocalizationProvider>
  </StrictMode>,
)

// Register Firebase Messaging service worker
if ('serviceWorker' in navigator) {
  const swUrl = new URL('firebase-messaging-sw.js', import.meta.env.BASE_URL).pathname
  navigator.serviceWorker.register(swUrl).catch(console.error)
}

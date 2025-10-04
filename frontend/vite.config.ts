import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// https://vite.dev/config/
export default defineConfig({
  base: '/client/',
  plugins: [react()],
  server: {
    proxy: {
      '/api': {
        // target: 'http://localhost:5000',
        target: 'http://ameenalqershi-001-site1.mtempurl.com',
        changeOrigin: true,
        secure: false,
      },
    },
  },
});

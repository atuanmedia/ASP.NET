import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

export default defineConfig({
  plugins: [react()],
  server: {
    proxy: {
      // Cấu hình proxy cho thư mục uploads
      '/uploads': {
        target: 'http://localhost:7075',
        changeOrigin: true,
        secure: false
      }
    }
  }
})
// import { useState } from 'react'
// import reactLogo from './assets/react.svg'
// import viteLogo from '/vite.svg'
// import './App.css'
import { RouterProvider } from 'react-router-dom'
import { router } from './router'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { useAuthStore } from '../features/auth/store/authStore';
import { useEffect } from 'react';

const queryClient = new QueryClient();

const AppContent = () => {
  const fetchUser = useAuthStore(s => s.fetchUser)

  useEffect(() => {
        fetchUser() // ⬅️ la fiecare refresh încarcă userul din cookie
    }, [])

  return (
    <RouterProvider router={router} />
  )
}

function App() {

  return (
    <QueryClientProvider client={queryClient}>
      <AppContent /> { }
    </QueryClientProvider>
  )
}

export default App

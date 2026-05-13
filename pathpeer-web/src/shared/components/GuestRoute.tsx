// src/shared/components/GuestRoute.tsx
import { Navigate } from 'react-router-dom'
import { useAuthStore } from '../../features/auth/store/authStore'

const GuestRoute = ({ children }: { children: React.ReactNode }) => {
    const { user, isLoading} = useAuthStore()

    if (isLoading) return null

    if (user) return <Navigate to="/" replace />

    return <>{children}</>
}

export default GuestRoute
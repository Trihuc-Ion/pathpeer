// src/shared/components/ProtectedRoute.tsx
import { Navigate } from 'react-router-dom'
import { useAuthStore } from '../../features/auth/store/authStore'

const ProtectedRoute = ({ children }: { children: React.ReactNode }) => {
    const { user, isLoading} = useAuthStore()

    if(isLoading) return null

    if (!user) return <Navigate to="/login" replace />

    return <>{children}</>
}

export default ProtectedRoute
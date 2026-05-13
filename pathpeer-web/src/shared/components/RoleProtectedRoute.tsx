// import { Navigate } from 'react-router-dom'
// import { useAuthStore } from '../features/auth/store/authStore'

// interface Props {
//     children: React.ReactNode
//     allowedRoles: string[]  // ['Admin'] sau ['Admin', 'Teacher']
// }

// const RoleProtectedRoute = ({ children, allowedRoles }: Props) => {
//     const { user, isLoading } = useAuthStore()

//     if (isLoading) return null

//     // Nu e logat
//     if (!user) return <Navigate to="/login" replace />

//     // E logat dar nu are rolul
//     if (!allowedRoles.includes(user.role)) return <Navigate to="/" replace />

//     return <>{children}</>
// }

// export default RoleProtectedRoute
import { useNavigate } from "react-router-dom"
import { useAuthStore } from "../features/auth/store/authStore"
import { authApi } from "../features/auth/api/authApi"
import { useMutation } from "@tanstack/react-query"
import type React from "react"

const Home = () => {
    const navigate = useNavigate()
    const { user , setUser} = useAuthStore()

    const { mutate: logout } = useMutation({
    mutationFn: authApi.logout,
    onSuccess: () => {
        setUser(null)
        navigate('/login')
    }
})

    const handleLogout = async (e : React.FormEvent) => {
        e.preventDefault()
        logout()      
    }

    return (
        <div className="home-container">
            <div className="home-card">
                <h1 className="home-title">PathPeer</h1>

                {user ? (
                    <div className="home-content">
                        <p>
                            Salut, <strong>{user.username}</strong> 👋
                        </p>
                        <p className="role">Rol: {user.role}</p>

                        <div className="button-group">
                            <button
                                className="btn primary"
                                onClick={() => navigate('/courses')}
                            >
                                Vezi Cursuri
                            </button>

                            <button
                                className="btn danger"
                                onClick={handleLogout}
                            >
                                Logout
                            </button>
                        </div>
                    </div>
                ) : (
                    <div className="home-content">
                        <p>Bine ai venit pe PathPeer 🚀</p>

                        <div className="button-group">
                            <button
                                className="btn primary"
                                onClick={() => navigate('/login')}
                            >
                                Login
                            </button>

                            <button
                                className="btn secondary"
                                onClick={() => navigate('/register')}
                            >
                                Register
                            </button>
                        </div>
                    </div>
                )}
            </div>
        </div>
    )
}

export default Home
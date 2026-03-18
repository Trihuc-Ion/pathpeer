import { useNavigate } from "react-router-dom"
import { useAuthStore } from "../features/auth/store/authStore"

const Home = () => {
    const navigate = useNavigate()
    const { user, logout } = useAuthStore()

    const handleLogout = () => {
        logout()
        navigate('/login')
    }

    return (
        <div style={{ maxWidth: 600, margin: '100px auto', padding: 20 }}>
            <h1>PathPeer</h1>

            {user ? (
                // Autentificat
                <div>
                    <p>Salut, <strong>{user.username}</strong>!</p>
                    <p>Rol: {user.role}</p>
                    <button onClick={() => navigate('/courses')}>
                        Vezi Cursuri
                    </button>
                    <button onClick={handleLogout} style={{ marginLeft: 10 }}>
                        Logout
                    </button>
                </div>
            ) : (
                // Neautentificat
                <div>
                    <p>Bine ai venit pe PathPeer!</p>
                    <button onClick={() => navigate('/login')}>
                        Login
                    </button>
                    <button onClick={() => navigate('/register')} style={{ marginLeft: 10 }}>
                        Register
                    </button>
                </div>
            )}
        </div>
    )
}

export default Home
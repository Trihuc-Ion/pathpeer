import { useNavigate } from "react-router-dom";
import { useAuthStore } from "../features/auth/store/authStore";
import { useState } from "react";
import client from "../shared/api/axiosClient";

const Login = () => {
    
    const navigate = useNavigate()
    const { setUser, setToken } = useAuthStore()

    const [email, setEmail] = useState('')
    const [password, setPassword] = useState('')
    const [error, setError] = useState('')
    const [loading, setLoading] = useState(false)

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault()
        setLoading(true)
        setError('')

        try {
            const response = await client.post('/auth/login', { email, password })
            setToken(response.data.token)
            setUser(response.data.user)
            navigate('/')
        } catch (err: any) {
            setError(err.response?.data?.message || 'Email sau parolă incorectă')
        } finally {
            setLoading(false)
        }
    }

    return (
        <div style={{ maxWidth: 400, margin: '100px auto', padding: 20 }}>
            <h1>Login PathPeer</h1>

            {error && <p style={{ color: 'red' }}>{error}</p>}

            <form onSubmit={handleSubmit}>
                <div style={{ marginBottom: 16 }}>
                    <label>Email</label>
                    <input
                        type="email"
                        value={email}
                        onChange={(e) => setEmail(e.target.value)}
                        style={{ display: 'block', width: '100%', padding: 8 }}
                        required
                    />
                </div>

                <div style={{ marginBottom: 16 }}>
                    <label>Parolă</label>
                    <input
                        type="password"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                        style={{ display: 'block', width: '100%', padding: 8 }}
                        required
                    />
                </div>

                <button
                    type="submit"
                    disabled={loading}
                    style={{ width: '100%', padding: 10 }}
                >
                    {loading ? 'Se încarcă...' : 'Login'}
                </button>
            </form>

            <p>Nu ai cont? <a href="/register">Înregistrează-te</a></p>
        </div>
    );
};

export default Login;
import { useNavigate } from "react-router-dom";
import { useAuthStore } from "../features/auth/store/authStore";
import { useState } from "react";
import { useMutation } from "@tanstack/react-query";
import { authApi } from "../features/auth/api/authApi";

const Login = () => {
    const navigate = useNavigate();
    const { setUser } = useAuthStore();

    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');

    const { mutate: login, isPending, error } = useMutation({
        mutationFn: authApi.login,
        onSuccess: (user) => {
            setUser(user);
            navigate('/');
        }             
    })

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        login({ email, password });
    };

    return (
        <div className="login-container">
            <div className="login-card">
                <h1 className="login-title">Login PathPeer</h1>

                {error && <div className="error-box">{error.message}</div>}

                <form onSubmit={handleSubmit} className="login-form">
                    <div className="form-group">
                        <label>Email</label>
                        <input
                            type="email"
                            value={email}
                            onChange={(e) => setEmail(e.target.value)}
                            placeholder="ex: ion@gmail.com"
                            required
                        />
                    </div>

                    <div className="form-group">
                        <label>Parolă</label>
                        <input
                            type="password"
                            value={password}
                            onChange={(e) => setPassword(e.target.value)}
                            placeholder="********"
                            required
                        />
                    </div>

                    <button
                        type="submit"
                        disabled={isPending}
                        className="btn primary full"
                    >
                        {isPending ? 'Se încarcă...' : 'Login'}
                    </button>
                </form>

                <p className="login-footer">
                    Nu ai cont?{" "}
                    <span onClick={() => navigate('/register')} className="link">
                        Înregistrează-te
                    </span>
                </p>
            </div>
        </div>
    );
};

export default Login;
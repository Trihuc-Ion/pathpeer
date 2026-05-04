import { useNavigate } from "react-router-dom";
import { useAuthStore } from "../features/auth/store/authStore";
import { useState } from "react";
import { authApi } from "../features/auth/api/authApi";
import { useMutation } from "@tanstack/react-query";

const Register = () => {
    const navigate = useNavigate();
    const { setUser } = useAuthStore();

    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [username, setUsername] = useState('');

    const { mutate: register, isPending, error } = useMutation({
        mutationFn: authApi.register,
        onSuccess: (user) => {
            setUser(user);
            navigate('/');
        }             
    })

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        register({ email, password, username });
    };

    return (
        <div className="register-container">
            <div className="register-card">
                <h1 className="register-title">Register PathPeer</h1>

                {error && <div className="error-box">{error.message}</div>}

                <form onSubmit={handleSubmit} className="register-form">

                    <div className="form-group">
                        <label>Username</label>
                        <input
                            type="text"
                            value={username}
                            onChange={(e) => setUsername(e.target.value)}
                            placeholder="ex: ion123"
                            required
                        />
                    </div>

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
                        {isPending ? 'Se încarcă...' : 'Înregistrează-te'}
                    </button>
                </form>

                <p className="register-footer">
                    Ai deja cont?{" "}
                    <span onClick={() => navigate('/login')} className="link">
                        Login
                    </span>
                </p>
            </div>
        </div>
    );
};

export default Register;
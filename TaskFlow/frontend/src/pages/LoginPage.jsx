import { useState } from 'react';
import { useAuth } from '../context/AuthContext.jsx';
import { Link } from 'react-router-dom';

function LoginPage() {
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState('');
    const { login } = useAuth();

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError('');
        const loginError = await login(username, password);

        if (loginError) {
            if (Array.isArray(loginError) && loginError.length > 0) {
                setError(loginError[0].description);
            }
            else {
                setError(loginError);
            }
        }
    };

    return (
        <div className="auth-page">
            <form onSubmit={handleSubmit} className="auth-form">
                <h2>Login</h2>
                <input type="text" value={username} onChange={(e) => setUsername(e.target.value)} placeholder="Username" required />
                <input type="password" value={password} onChange={(e) => setPassword(e.target.value)} placeholder="Password" required />
                {error && <p className="error-message">{error}</p>}
                <button type="submit">Login</button>
                <p>Don't have an account? <Link to="/register">Register here</Link></p>
            </form>
        </div>
    );
}

export default LoginPage;
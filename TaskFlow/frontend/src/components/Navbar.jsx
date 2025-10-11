import { useAuth } from '../context/AuthContext.jsx';

function Navbar() {
    const { user, logout } = useAuth();
    const storedUser = user || JSON.parse(localStorage.getItem('user'));

    return (
        <nav className="navbar">
            <div className="navbar-brand">TaskFlow</div>
            {storedUser && (
                <div className="navbar-user">
                    <span>Welcome, {storedUser.username}!</span>
                    <button onClick={logout} className="logout-button">Logout</button>
                </div>
            )}
        </nav>
    );
}

export default Navbar;
import { Navigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext.jsx';

function ProtectedRoute({ children }) {
  const { user } = useAuth();

  const storedUser = JSON.parse(localStorage.getItem('user'));

  if (!user && !storedUser) {
    return <Navigate to="/login" />;
  }

  return children;
}

export default ProtectedRoute;
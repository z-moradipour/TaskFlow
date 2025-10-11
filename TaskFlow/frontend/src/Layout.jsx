// src/Layout.jsx
import { Outlet } from 'react-router-dom';
import { AuthProvider } from './context/AuthContext.jsx';
import Navbar from './components/Navbar.jsx'; // وارد کردن Navbar

function Layout() {
  return (
    <AuthProvider>
      {}
      <Navbar />
      {}
      <main>
        <Outlet />
      </main>
    </AuthProvider>
  );
}

export default Layout;
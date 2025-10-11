import React from 'react';
import ReactDOM from 'react-dom/client';
import { createBrowserRouter, RouterProvider } from 'react-router-dom';

import Layout from './Layout.jsx';
import ProtectedRoute from './components/ProtectedRoute.jsx';
import App from './App.jsx';
import BoardPage from './pages/BoardPage.jsx';
import LoginPage from './pages/LoginPage.jsx';
import RegisterPage from './pages/RegisterPage.jsx';
import './index.css';

const router = createBrowserRouter([
  {
    element: <Layout />, 
    children: [
      {
        path: "/",
        element: (
          <ProtectedRoute>
            <App />
          </ProtectedRoute>
        ),
      },
      {
        path: "/board/:boardId",
        element: (
          <ProtectedRoute>
            <BoardPage />
          </ProtectedRoute>
        ),

      },
      {
        path: "/login",
        element: <LoginPage />,
      },
      {
        path: "/register",
        element: <RegisterPage />,
      },
    ]
  }
]);

ReactDOM.createRoot(document.getElementById('root')).render(
  <React.StrictMode>
    <RouterProvider router={router} />
  </React.StrictMode>
);
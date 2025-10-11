import React, { createContext, useState, useContext } from 'react';
import axios from 'axios';
import { useNavigate } from 'react-router-dom';

const AuthContext = createContext(null);

export const useAuth = () => useContext(AuthContext);

export const AuthProvider = ({ children }) => {
    const [user, setUser] = useState(null);
    const navigate = useNavigate();

    const login = async (username, password) => {
        try {
            const response = await axios.post('https://localhost:7289/api/account/login', { username, password });
            setUser(response.data);
            localStorage.setItem('user', JSON.stringify(response.data));
            navigate('/'); 
            return null; 
        } catch (error) {
            console.error('Login failed:', error);
            return error.response?.data || "Login failed";
        }
    };

    const register = async (email, username, password) => {
        try {
            const response = await axios.post('https://localhost:7289/api/account/register', { email, username, password });
            setUser(response.data);
            localStorage.setItem('user', JSON.stringify(response.data));
            navigate('/');
            return null;
        } catch (error) {
            console.error('Registration failed:', error);
            return error.response?.data || "Registration failed";
        }
    };

    const logout = () => {
        setUser(null);
        localStorage.removeItem('user');
        navigate('/login');
    };

    const value = { user, login, register, logout };

    return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};
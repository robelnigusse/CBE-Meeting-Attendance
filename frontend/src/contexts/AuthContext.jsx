import { createContext, useContext, useState, useEffect } from 'react';
import api from '../api/axios';
import { useNavigate } from 'react-router-dom';

const AuthContext = createContext();

export const useAuth = () => {
  return useContext(AuthContext);
};

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);


  useEffect(() => {
    // Check if user is logged in on mount
    const storedUser = localStorage.getItem('user');
    const token = localStorage.getItem('accessToken');

    if (storedUser && token) {
      setUser(JSON.parse(storedUser));
    }
    setLoading(false);
  }, []);



  const login = async (email, password) => {
    const response = await api.post('/users/login', { email, password });

    // Check if the backend ApiResponse indicates success
    if (!response.data.success) {
      throw new Error(response.data.message || 'Invalid login credentials');
    }

    const { token, user: userData } = response.data.data;

    localStorage.setItem('accessToken', token);

    const loggedUser = {
      ...userData,
      roles: userData.roles || ['Admin']
    };

    localStorage.setItem('user', JSON.stringify(loggedUser));
    setUser(loggedUser);
    return response.data;
  };

  const logout = () => {
    // Ideally we also hit the /api/users/logout endpoint here
    localStorage.removeItem('accessToken');
    localStorage.removeItem('refreshToken');
    localStorage.removeItem('user');
    setUser(null);
  };

  const value = {
    user,
    login,
    logout,
    loading
  };

  return (
    <AuthContext.Provider value={value}>
      {!loading && children}
    </AuthContext.Provider>
  );
};

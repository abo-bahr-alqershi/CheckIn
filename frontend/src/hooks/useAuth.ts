import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { CommonUsersService } from '../services/common-users.service';
import { CommonAuthService } from '../services/common-auth.service';
import type { RefreshTokenCommand } from '../types/auth.types';

interface AuthUser {
  id: string;
  name: string;
  email: string;
  role: string;
  phone?: string;
  profileImage?: string;
  propertyId?: string;
  propertyName?: string;
  staffId?: string;
  settingsJson?: string;
  favoritesJson?: string;
}

interface UseAuthReturn {
  user: AuthUser | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  login: (token: string, refreshToken: string, userData: AuthUser) => void;
  logout: () => void;
  updateUser: (userData: Partial<AuthUser>) => void;
}

export const useAuth = (): UseAuthReturn => {
  const [user, setUser] = useState<AuthUser | null>(null);
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [isLoading, setIsLoading] = useState(true);
  const navigate = useNavigate();

  useEffect(() => {
    const initializeAuth = async () => {
      const token = localStorage.getItem('token');
      const refreshToken = localStorage.getItem('refreshToken');
      const userData = localStorage.getItem('user');

      if (token && userData) {
        const parsedUser = JSON.parse(userData); // استخدم بيانات المستخدم المخزنة محليًا للحفاظ على الدور
        try {
          // التحقق من صحة التوكن مع الخادم (لا حاجة لاستخدام result.data لبيانات المستخدم)
          const result = await CommonUsersService.getCurrentUser({});
          if (result.success) {
            setUser(parsedUser);
            setIsAuthenticated(true);
          } else {
            // محاولة تحديث التوكن إذا كان متوفراً
            if (refreshToken) {
              await attemptTokenRefresh(refreshToken);
            } else {
              clearAuthData();
            }
          }
        } catch (error) {
          console.error('خطأ في التحقق من الهوية:', error);
          // محاولة تحديث التوكن في حالة الخطأ
          if (refreshToken) {
            await attemptTokenRefresh(refreshToken);
          } else {
            clearAuthData();
          }
        }
      }
      
      setIsLoading(false);
    };

    initializeAuth();
  }, []);

  const attemptTokenRefresh = async (refreshToken: string) => {
    try {
      const refreshCommand: RefreshTokenCommand = { refreshToken };
      const result = await CommonAuthService.refreshToken(refreshCommand);
      
      if (result.success && result.data) {
        const auth = result.data;
        const newUser: AuthUser = {
          id: auth.userId,
          name: auth.userName,
          email: auth.email,
          role: auth.role,
          profileImage: auth.profileImage,
          propertyId: auth.propertyId,
          propertyName: auth.propertyName,
          staffId: auth.staffId,
          settingsJson: auth.settingsJson,
          favoritesJson: auth.favoritesJson,
        };
        localStorage.setItem('token', auth.accessToken);
        localStorage.setItem('refreshToken', auth.refreshToken);
        localStorage.setItem('user', JSON.stringify(newUser));
        setUser(newUser);
        setIsAuthenticated(true);
      } else {
        clearAuthData();
      }
    } catch (error) {
      console.error('خطأ في تحديث التوكن:', error);
      clearAuthData();
    }
  };

  const clearAuthData = () => {
    localStorage.removeItem('token');
    localStorage.removeItem('refreshToken');
    localStorage.removeItem('user');
    setUser(null);
    setIsAuthenticated(false);
  };

  const login = (token: string, refreshToken: string, userData: AuthUser) => {
    localStorage.setItem('token', token);
    localStorage.setItem('refreshToken', refreshToken);
    localStorage.setItem('user', JSON.stringify(userData));
    
    setUser(userData);
    setIsAuthenticated(true);
  };

  const logout = () => {
    clearAuthData();
    navigate('/auth/login');
  };

  const updateUser = (userData: Partial<AuthUser>) => {
    if (user) {
      const updatedUser = { ...user, ...userData };
      setUser(updatedUser);
      localStorage.setItem('user', JSON.stringify(updatedUser));
    }
  };

  return {
    user,
    isAuthenticated,
    isLoading,
    login,
    logout,
    updateUser
  };
};
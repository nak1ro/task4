import { useState, useCallback, useEffect } from 'react';
import { User, AuthResponse } from '../types';
import { authApi, LoginRequest, RegisterRequest } from '../api/authApi';

export const useAuth = () => {
    const [user, setUser] = useState<User | null>(null);
    const [isAuthenticated, setIsAuthenticated] = useState<boolean>(false);
    const [isLoading, setIsLoading] = useState<boolean>(true);
    const [error, setError] = useState<string | null>(null);

    const checkAuthStatus = useCallback(async () => {
        setIsLoading(true);
        try {
            const response = await authApi.getMe();
            setUser(response.data);
            setIsAuthenticated(true);
        } catch (err) {
            setUser(null);
            setIsAuthenticated(false);
        } finally {
            setIsLoading(false);
        }
    }, []);

    useEffect(() => {
        checkAuthStatus();
    }, [checkAuthStatus]);

    const login = async (data: LoginRequest) => {
        setIsLoading(true);
        setError(null);
        try {
            await authApi.login(data);
            await checkAuthStatus();
        } catch (err: any) {
            setError(err.response?.data?.message || 'Login failed');
            throw err;
        } finally {
            setIsLoading(false);
        }
    };

    const register = async (data: RegisterRequest) => {
        setIsLoading(true);
        setError(null);
        try {
            await authApi.register(data);
            // Registration doesn't auto-login usually, but if it did, we'd checkAuthStatus here.
            // Requirement: "Users are registered right away ... message is shown".
            // So we just return success.
        } catch (err: any) {
            setError(err.response?.data?.message || 'Registration failed');
            throw err;
        } finally {
            setIsLoading(false);
        }
    };

    const logout = async () => {
        setIsLoading(true);
        try {
            await authApi.logout();
            setUser(null);
            setIsAuthenticated(false);
        } catch (err) {
            console.error('Logout failed', err);
        } finally {
            setIsLoading(false);
        }
    };

    return {
        user,
        isAuthenticated,
        isLoading,
        error,
        login,
        register,
        logout,
        checkAuthStatus
    };
};

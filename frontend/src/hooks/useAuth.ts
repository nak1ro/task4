import { useState, useCallback, useEffect } from 'react';
import type { User } from '../types';
import type { LoginRequest, RegisterRequest } from '../api/authApi';
import { authApi } from '../api/authApi';

export const useAuth = () => {
    const [user, setUser] = useState<User | null>(null);
    const [isAuthenticated, setIsAuthenticated] = useState<boolean>(false);
    const [isLoading, setIsLoading] = useState<boolean>(true);
    const [error, setError] = useState<string | null>(null);

    const checkAuthStatus = useCallback(async () => {
        setIsLoading(true);
        try {
            const response = await authApi.getMe();
            // Convert MeResponse to User format for frontend state
            const [firstName, ...lastNameParts] = (response.data.name || '').split(' ');
            setUser({
                id: response.data.id,
                firstName: firstName || '',
                lastName: lastNameParts.join(' ') || '',
                email: response.data.email,
                status: 'Active',
                registrationTime: new Date().toISOString(),
            });
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

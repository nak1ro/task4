import client from './client';
import { User, AuthResponse } from '../types';

export interface RegisterRequest {
    firstName: string;
    lastName: string;
    email: string;
    password: string;
}

export interface LoginRequest {
    email: string;
    password: string;
}

export const authApi = {
    register: async (data: RegisterRequest) => {
        return client.post<AuthResponse>('/auth/register', data);
    },

    login: async (data: LoginRequest) => {
        return client.post<AuthResponse>('/auth/login', data);
    },

    logout: async () => {
        return client.post<AuthResponse>('/auth/logout');
    },

    getMe: async () => {
        return client.get<User>('/auth/me');
    },

    confirmEmail: async (token: string) => {
        return client.get<AuthResponse>(`/auth/confirm-email?token=${token}`);
    }
};

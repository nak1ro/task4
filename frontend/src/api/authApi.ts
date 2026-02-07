import client from './client';
import type { AuthResponse } from '../types';

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

export interface MeResponse {
    id: string;
    email: string;
    name: string;
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
        return client.get<MeResponse>('/auth/me');
    },

    confirmEmail: async (token: string) => {
        return client.get<AuthResponse>(`/auth/confirm-email?token=${token}`);
    }
};

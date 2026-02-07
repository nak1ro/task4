import client from './client';
import type { User } from '../types';

export const usersApi = {
    getAll: async () => {
        return client.get<User[]>('/users');
    },

    block: async (userIds: string[]) => {
        return client.post('/users/block', { userIds });
    },

    unblock: async (userIds: string[]) => {
        return client.post('/users/unblock', { userIds });
    },

    delete: async (userIds: string[]) => {
        return client.delete('/users', { data: { userIds } });
    },

    deleteUnverified: async (userIds: string[]) => {
        return client.delete('/users/unverified', { data: { userIds } });
    }
};

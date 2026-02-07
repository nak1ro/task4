import { useState, useCallback } from 'react';
import { User } from '../types';
import { usersApi } from '../api/usersApi';

export const useUsers = () => {
    const [users, setUsers] = useState<User[]>([]);
    const [isLoading, setIsLoading] = useState<boolean>(false);
    const [error, setError] = useState<string | null>(null);
    const [selectedIds, setSelectedIds] = useState<Set<string>>(new Set());

    const fetchUsers = useCallback(async () => {
        setIsLoading(true);
        try {
            const response = await usersApi.getAll();
            setUsers(response.data);
        } catch (err: any) {
            setError(err.response?.data?.message || 'Failed to fetch users');
        } finally {
            setIsLoading(false);
        }
    }, []);

    const toggleSelect = (id: string) => {
        const newSelected = new Set(selectedIds);
        if (newSelected.has(id)) {
            newSelected.delete(id);
        } else {
            newSelected.add(id);
        }
        setSelectedIds(newSelected);
    };

    const toggleSelectAll = (selectAll: boolean) => {
        if (selectAll) {
            setSelectedIds(new Set(users.map(u => u.id)));
        } else {
            setSelectedIds(new Set());
        }
    };

    const performAction = async (action: (ids: string[]) => Promise<any>) => {
        if (selectedIds.size === 0) return;

        setIsLoading(true);
        try {
            await action(Array.from(selectedIds));
            await fetchUsers(); // Refresh list
            setSelectedIds(new Set()); // Clear selection
        } catch (err: any) {
            setError(err.response?.data?.message || 'Action failed');
            // Check if we need to redirect due to block/delete of self?
            // The API interceptor/middleware handles 401 redirects.
            if (err.response?.status === 401) {
                window.location.reload(); // Simple way to trigger auth check
            }
        } finally {
            setIsLoading(false);
        }
    };

    const blockUsers = () => performAction(usersApi.block);
    const unblockUsers = () => performAction(usersApi.unblock);
    const deleteUsers = () => performAction(usersApi.delete);
    const deleteUnverifiedUsers = () => performAction(usersApi.deleteUnverified);

    return {
        users,
        isLoading,
        error,
        selectedIds,
        fetchUsers,
        toggleSelect,
        toggleSelectAll,
        blockUsers,
        unblockUsers,
        deleteUsers,
        deleteUnverifiedUsers
    };
};

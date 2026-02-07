import { useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useUsers } from '../hooks/useUsers';
import { useAuth } from '../hooks/useAuth';
import { UserToolbar } from '../features/users/UserToolbar';
import { UserTable } from '../features/users/UserTable';
import { Button } from '../components';

export const UsersPage = () => {
    const navigate = useNavigate();
    const { user, logout } = useAuth();
    const {
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
    } = useUsers();

    useEffect(() => {
        fetchUsers();
    }, [fetchUsers]);

    const handleLogout = async () => {
        await logout();
        navigate('/auth');
    };

    return (
        <div className="min-h-screen bg-gray-50">
            <header className="bg-white shadow">
                <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-4 flex justify-between items-center">
                    <div>
                        <h1 className="text-2xl font-bold text-gray-900">User Management</h1>
                        {user && (
                            <p className="text-sm text-gray-600">
                                Logged in as: {user.firstName} {user.lastName} ({user.email})
                            </p>
                        )}
                    </div>
                    <Button onClick={handleLogout} variant="secondary">
                        Logout
                    </Button>
                </div>
            </header>

            <main className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
                <div className="bg-white rounded-lg shadow">
                    <UserToolbar
                        selectedCount={selectedIds.size}
                        onBlock={blockUsers}
                        onUnblock={unblockUsers}
                        onDelete={deleteUsers}
                        onDeleteUnverified={deleteUnverifiedUsers}
                        disabled={isLoading}
                    />

                    {error && (
                        <div className="p-4 bg-red-50 border-b border-red-200 text-red-600">
                            {error}
                        </div>
                    )}

                    {isLoading ? (
                        <div className="p-8 text-center text-gray-500">Loading users...</div>
                    ) : (
                        <UserTable
                            users={users}
                            selectedIds={selectedIds}
                            onToggleSelect={toggleSelect}
                            onToggleSelectAll={toggleSelectAll}
                        />
                    )}
                </div>
            </main>
        </div>
    );
};

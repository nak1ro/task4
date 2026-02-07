import { User } from '../../types';
import { Checkbox, Badge, Table, TableHeader, TableBody, TableRow, TableHead, TableCell } from '../../components';

interface UserTableProps {
    users: User[];
    selectedIds: Set<string>;
    onToggleSelect: (id: string) => void;
    onToggleSelectAll: (selectAll: boolean) => void;
}

export const UserTable = ({ users, selectedIds, onToggleSelect, onToggleSelectAll }: UserTableProps) => {
    const allSelected = users.length > 0 && users.every(u => selectedIds.has(u.id));
    const someSelected = users.some(u => selectedIds.has(u.id)) && !allSelected;

    const formatDate = (dateString?: string) => {
        if (!dateString) return 'Never';
        return new Date(dateString).toLocaleString();
    };

    const getBadgeVariant = (status: string): 'active' | 'unverified' | 'blocked' => {
        return status.toLowerCase() as 'active' | 'unverified' | 'blocked';
    };

    return (
        <Table>
            <TableHeader>
                <TableRow>
                    <TableHead className="w-12">
                        <Checkbox
                            checked={allSelected}
                            onChange={(e) => onToggleSelectAll(e.target.checked)}
                            ref={(el) => {
                                if (el) {
                                    el.indeterminate = someSelected;
                                }
                            }}
                        />
                    </TableHead>
                    <TableHead>Name</TableHead>
                    <TableHead>Email</TableHead>
                    <TableHead>Last Login Time</TableHead>
                    <TableHead>Registration Time</TableHead>
                    <TableHead>Status</TableHead>
                </TableRow>
            </TableHeader>
            <TableBody>
                {users.length === 0 ? (
                    <TableRow>
                        <TableCell colSpan={6} className="text-center text-gray-500">
                            No users found
                        </TableCell>
                    </TableRow>
                ) : (
                    users.map((user) => (
                        <TableRow key={user.id}>
                            <TableCell>
                                <Checkbox
                                    checked={selectedIds.has(user.id)}
                                    onChange={() => onToggleSelect(user.id)}
                                />
                            </TableCell>
                            <TableCell>
                                {user.firstName} {user.lastName}
                            </TableCell>
                            <TableCell className="text-gray-600">{user.email}</TableCell>
                            <TableCell className="text-gray-600">{formatDate(user.lastLoginTime)}</TableCell>
                            <TableCell className="text-gray-600">{formatDate(user.registrationTime)}</TableCell>
                            <TableCell>
                                <Badge variant={getBadgeVariant(user.status)}>
                                    {user.status}
                                </Badge>
                            </TableCell>
                        </TableRow>
                    ))
                )}
            </TableBody>
        </Table>
    );
};

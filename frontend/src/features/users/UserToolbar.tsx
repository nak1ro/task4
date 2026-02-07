import { LockOpenIcon, TrashIcon, UserMinusIcon } from '@heroicons/react/24/outline';
import { Button, IconButton, Tooltip } from '../../components';

interface UserToolbarProps {
    selectedCount: number;
    onBlock: () => void;
    onUnblock: () => void;
    onDelete: () => void;
    onDeleteUnverified: () => void;
    disabled: boolean;
}

export const UserToolbar = ({
    selectedCount,
    onBlock,
    onUnblock,
    onDelete,
    onDeleteUnverified,
    disabled
}: UserToolbarProps) => {
    const hasSelection = selectedCount > 0;

    return (
        <div className="bg-white p-4 border-b border-gray-200 flex items-center gap-2">
            <div className="text-sm text-gray-600 mr-4">
                {selectedCount > 0 ? `${selectedCount} selected` : 'No users selected'}
            </div>

            <Button
                onClick={onBlock}
                disabled={!hasSelection || disabled}
                variant="secondary"
            >
                Block
            </Button>

            <Tooltip content="Unblock">
                <IconButton
                    onClick={onUnblock}
                    disabled={!hasSelection || disabled}
                    icon={<LockOpenIcon className="w-5 h-5" />}
                />
            </Tooltip>

            <Tooltip content="Delete">
                <IconButton
                    onClick={onDelete}
                    disabled={!hasSelection || disabled}
                    variant="danger"
                    icon={<TrashIcon className="w-5 h-5" />}
                />
            </Tooltip>

            <Tooltip content="Delete Unverified">
                <IconButton
                    onClick={onDeleteUnverified}
                    disabled={!hasSelection || disabled}
                    variant="danger"
                    icon={<UserMinusIcon className="w-5 h-5" />}
                />
            </Tooltip>
        </div>
    );
};

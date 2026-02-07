import type { ReactNode } from 'react';

interface BadgeProps {
    variant: 'active' | 'unverified' | 'blocked';
    children: ReactNode;
}

export const Badge = ({ variant, children }: BadgeProps) => {
    const variantStyles = {
        active: 'bg-green-100 text-green-800',
        unverified: 'bg-yellow-100 text-yellow-800',
        blocked: 'bg-red-100 text-red-800'
    };

    return (
        <span className={`px-2 py-1 text-xs font-medium rounded-full ${variantStyles[variant]}`}>
            {children}
        </span>
    );
};

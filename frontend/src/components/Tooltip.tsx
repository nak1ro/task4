import { type ReactNode, useState } from 'react';

interface TooltipProps {
    content: string;
    children: ReactNode;
}

export const Tooltip = ({ content, children }: TooltipProps) => {
    const [visible, setVisible] = useState(false);

    return (
        <div
            className="relative inline-block"
            onMouseEnter={() => setVisible(true)}
            onMouseLeave={() => setVisible(false)}
        >
            {children}
            {visible && (
                <div className="absolute z-10 px-3 py-2 text-sm font-medium text-white bg-gray-900 rounded-lg shadow-sm -top-2 -translate-y-full left-1/2 -translate-x-1/2 whitespace-nowrap">
                    {content}
                    <div className="absolute w-2 h-2 bg-gray-900 rotate-45 left-1/2 -translate-x-1/2 -bottom-1"></div>
                </div>
            )}
        </div>
    );
};

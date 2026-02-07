import { type InputHTMLAttributes, forwardRef } from 'react';

interface CheckboxProps extends Omit<InputHTMLAttributes<HTMLInputElement>, 'type'> {
    label?: string;
}

export const Checkbox = forwardRef<HTMLInputElement, CheckboxProps>(
    ({ label, className = '', ...props }, ref) => {
        return (
            <label className="flex items-center cursor-pointer">
                <input
                    ref={ref}
                    type="checkbox"
                    className={`w-4 h-4 text-blue-600 border-gray-300 rounded focus:ring-blue-500 cursor-pointer ${className}`}
                    {...props}
                />
                {label && (
                    <span className="ml-2 text-sm text-gray-700">{label}</span>
                )}
            </label>
        );
    }
);

Checkbox.displayName = 'Checkbox';

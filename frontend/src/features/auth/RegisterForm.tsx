import { useState, type FormEvent } from 'react';
import { Input, Button } from '../../components';

interface RegisterFormProps {
    onSubmit: (firstName: string, lastName: string, email: string, password: string) => Promise<void>;
    isLoading: boolean;
    error: string | null;
    successMessage: string | null;
}

export const RegisterForm = ({ onSubmit, isLoading, error, successMessage }: RegisterFormProps) => {
    const [firstName, setFirstName] = useState('');
    const [lastName, setLastName] = useState('');
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');

    const handleSubmit = async (e: FormEvent) => {
        e.preventDefault();
        await onSubmit(firstName, lastName, email, password);
    };

    return (
        <form onSubmit={handleSubmit} className="space-y-4">
            <Input
                label="First Name"
                type="text"
                value={firstName}
                onChange={(e) => setFirstName(e.target.value)}
                required
                disabled={isLoading}
            />
            <Input
                label="Last Name"
                type="text"
                value={lastName}
                onChange={(e) => setLastName(e.target.value)}
                required
                disabled={isLoading}
            />
            <Input
                label="Email"
                type="email"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                required
                disabled={isLoading}
            />
            <Input
                label="Password"
                type="password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                required
                disabled={isLoading}
            />
            {error && (
                <div className="text-red-600 text-sm">{error}</div>
            )}
            {successMessage && (
                <div className="text-green-600 text-sm">{successMessage}</div>
            )}
            <Button type="submit" variant="primary" className="w-full" disabled={isLoading}>
                {isLoading ? 'Registering...' : 'Register'}
            </Button>
        </form>
    );
};

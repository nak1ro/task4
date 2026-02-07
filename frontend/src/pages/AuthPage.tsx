import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../hooks/useAuth';
import { LoginForm } from '../features/auth/LoginForm';
import { RegisterForm } from '../features/auth/RegisterForm';
import { Button } from '../components';

export const AuthPage = () => {
    const [isLogin, setIsLogin] = useState(true);
    const [successMessage, setSuccessMessage] = useState<string | null>(null);
    const navigate = useNavigate();
    const { login, register, isLoading, error } = useAuth();

    const handleLogin = async (email: string, password: string) => {
        try {
            await login({ email, password });
            navigate('/users');
        } catch (err) {
            // Error is handled by useAuth hook
        }
    };

    const handleRegister = async (firstName: string, lastName: string, email: string, password: string) => {
        try {
            await register({ firstName, lastName, email, password });
            setSuccessMessage('Registration successful! Please check your email to confirm your account.');
        } catch (err) {
            // Error is handled by useAuth hook
        }
    };

    return (
        <div className="min-h-screen flex items-center justify-center bg-gray-50 px-4">
            <div className="max-w-md w-full space-y-6">
                <div className="text-center">
                    <h1 className="text-3xl font-bold text-gray-900">User Management</h1>
                    <p className="mt-2 text-sm text-gray-600">
                        {isLogin ? 'Login to your account' : 'Create a new account'}
                    </p>
                </div>

                <div className="bg-white p-8 rounded-lg shadow">
                    <div className="flex mb-6 border-b">
                        <button
                            onClick={() => {
                                setIsLogin(true);
                                setSuccessMessage(null);
                            }}
                            className={`flex-1 pb-2 text-center font-medium transition-colors ${isLogin
                                    ? 'border-b-2 border-blue-600 text-blue-600'
                                    : 'text-gray-500 hover:text-gray-700'
                                }`}
                        >
                            Login
                        </button>
                        <button
                            onClick={() => {
                                setIsLogin(false);
                                setSuccessMessage(null);
                            }}
                            className={`flex-1 pb-2 text-center font-medium transition-colors ${!isLogin
                                    ? 'border-b-2 border-blue-600 text-blue-600'
                                    : 'text-gray-500 hover:text-gray-700'
                                }`}
                        >
                            Register
                        </button>
                    </div>

                    {isLogin ? (
                        <LoginForm
                            onSubmit={handleLogin}
                            isLoading={isLoading}
                            error={error}
                        />
                    ) : (
                        <RegisterForm
                            onSubmit={handleRegister}
                            isLoading={isLoading}
                            error={error}
                            successMessage={successMessage}
                        />
                    )}
                </div>
            </div>
        </div>
    );
};

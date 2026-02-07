import { useEffect, useState, useRef } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';
import { authApi } from '../api/authApi';
import { Button } from '../components';

export const ConfirmEmailPage = () => {
    const [searchParams] = useSearchParams();
    const navigate = useNavigate();
    const [status, setStatus] = useState<'loading' | 'success' | 'error'>('loading');
    const [message, setMessage] = useState('');

    const effectRan = useRef(false);

    useEffect(() => {
        if (effectRan.current) return;

        const confirmEmail = async () => {
            const token = searchParams.get('token');

            if (!token) {
                setStatus('error');
                setMessage('Invalid confirmation link. No token provided.');
                return;
            }

            // Mark as ran to prevent double execution in Strict Mode
            effectRan.current = true;

            try {
                const response = await authApi.confirmEmail(token);
                setStatus('success');
                setMessage(response.data.message || 'Email confirmed successfully!');
            } catch (err: any) {
                setStatus('error');
                setMessage(err.response?.data?.message || 'Failed to confirm email.');
            }
        };

        confirmEmail();
    }, [searchParams]);

    return (
        <div className="min-h-screen flex items-center justify-center bg-gray-50 px-4">
            <div className="max-w-md w-full bg-white p-8 rounded-lg shadow text-center">
                <h1 className="text-2xl font-bold text-gray-900 mb-4">Email Confirmation</h1>

                {status === 'loading' && (
                    <p className="text-gray-600">Confirming your email...</p>
                )}

                {status === 'success' && (
                    <>
                        <div className="text-green-600 mb-4">
                            <svg className="w-16 h-16 mx-auto mb-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 13l4 4L19 7" />
                            </svg>
                            <p>{message}</p>
                        </div>
                        <Button onClick={() => navigate('/auth')} variant="primary">
                            Go to Login
                        </Button>
                    </>
                )}

                {status === 'error' && (
                    <>
                        <div className="text-red-600 mb-4">
                            <svg className="w-16 h-16 mx-auto mb-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
                            </svg>
                            <p>{message}</p>
                        </div>
                        <Button onClick={() => navigate('/auth')} variant="secondary">
                            Back to Login
                        </Button>
                    </>
                )}
            </div>
        </div>
    );
};

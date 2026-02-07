import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { AuthPage } from '../pages/AuthPage';
import { UsersPage } from '../pages/UsersPage';
import { ConfirmEmailPage } from '../pages/ConfirmEmailPage';
import { ProtectedRoute } from './ProtectedRoute';

export const Router = () => {
    return (
        <BrowserRouter>
            <Routes>
                <Route path="/auth" element={<AuthPage />} />
                <Route path="/confirm-email" element={<ConfirmEmailPage />} />
                <Route
                    path="/users"
                    element={
                        <ProtectedRoute>
                            <UsersPage />
                        </ProtectedRoute>
                    }
                />
                <Route path="/" element={<Navigate to="/users" replace />} />
                <Route path="*" element={<Navigate to="/users" replace />} />
            </Routes>
        </BrowserRouter>
    );
};

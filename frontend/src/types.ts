export interface User {
    id: string;
    firstName: string;
    lastName: string;
    email: string;
    status: 'Active' | 'Unverified' | 'Blocked';
    lastLoginTime?: string;
    registrationTime: string;
}

export interface AuthResponse {
    message: string;
}

export interface ApiError {
    message: string;
}

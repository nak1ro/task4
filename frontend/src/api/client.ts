import axios from 'axios';

const client = axios.create({
    baseURL: 'http://localhost:5000/api',
    withCredentials: true,
    headers: {
        'Content-Type': 'application/json',
    },
});

// Optional: Add response interceptor for global error handling (e.g., 401 redirects)
client.interceptors.response.use(
    (response) => response,
    (error) => {
        if (error.response && error.response.status === 401) {
            // Handle unauthorized access (e.g., redirect to login)
            // For now, we just pass the error through to be handled by the caller or hooks
            if (window.location.pathname !== '/auth') {
                // Simple redirect for now, routing logic will be robust in Router.tsx
                // window.location.href = '/auth'; 
                // Better to let the component/hook handle it to avoid loops
            }
        }
        return Promise.reject(error);
    }
);

export default client;

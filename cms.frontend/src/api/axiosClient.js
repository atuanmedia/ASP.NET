import axios from 'axios';

const axiosClient = axios.create({
    baseURL: 'https://localhost:7075/api',
    headers: {
        'Content-Type': 'application/json',
    },
    timeout: 10000,
});

// Đính kèm JWT token vào mỗi request nếu có
axiosClient.interceptors.request.use((config) => {
    const token = localStorage.getItem('auth_token');
    if (token) {
        config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
});

axiosClient.interceptors.response.use(
    (response) => response.data,
    (error) => {
        console.error('Lỗi kết nối API:', error.message);
        return Promise.reject(error);
    }
);

export default axiosClient;

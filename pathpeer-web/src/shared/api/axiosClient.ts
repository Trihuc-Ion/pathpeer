import axios from "axios";
import { useAuthStore } from "../../features/auth/store/authStore";

const client = axios.create({
  baseURL: 'http://localhost:5164/api',
});

client.interceptors.request.use((config) => {
    const token = useAuthStore.getState().token;
    if (token) {
        config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
});

export default client;

import axios from "axios";
// import { useAuthStore } from "../../features/auth/store/authStore";

console.log('API URL:', import.meta.env.VITE_API_URL);

const client = axios.create({
  baseURL: `${import.meta.env.VITE_API_URL}/api`,
  withCredentials: true,
});

export default client;

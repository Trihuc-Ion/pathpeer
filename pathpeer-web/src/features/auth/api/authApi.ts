import client from '../../../shared/api/axiosClient';
import type { User } from '../../../shared/types';

interface LoginDto {
    email: string;
    password: string;
}

interface RegisterDto {
    email: string;
    password: string;
    username: string;
}

export const authApi = {
    login: (dto: LoginDto): Promise<User> =>
        client.post('/auth/login', dto).then(r => r.data),

    register: (dto: RegisterDto): Promise<User> =>
        client.post('/auth/register', dto).then(r => r.data),

    logout: (): Promise<void> =>
        client.post('/auth/logout').then(r => r.data),

    getProfile: (): Promise<User> =>
        client.get('/auth/profile').then(r => r.data),
}
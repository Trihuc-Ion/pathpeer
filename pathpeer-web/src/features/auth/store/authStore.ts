import { create } from 'zustand'
import type { User } from '../../../shared/types';
import client from '../../../shared/api/axiosClient';

interface AuthState {
    user: User | null;
    isLoading: boolean;
    setUser: (user: User | null) => void;
    fetchUser: () => Promise<void>;
    logout: () => Promise<void>;
}

export const useAuthStore = create<AuthState>((set) => ({
    user: null,
    isLoading: true,

    setUser: (user) => {
        return set({ user })
    },

    fetchUser: async () => {
        try {
            const res = await client.get('/auth/profile');
            set({ user: res.data, isLoading: false });
        } catch {
            set({ user: null, isLoading: false });
        }
    },

    logout: async () => {
        await client.post('/auth/logout');
        set({ user: null });
    }
}))
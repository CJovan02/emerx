import { create } from 'zustand';
import type AppUser from '../domain/models/appUser.ts';

interface UserState {
	user: AppUser | null;
	isLoading: boolean;
	setLoading: (value: boolean) => void;
	setUser: (user: AppUser) => void;
	clearUser: () => void;
}

export const useUserStore = create<UserState>(set => ({
	user: null,
	isLoading: true,
	setLoading: value => set({ isLoading: value }),
	setUser: user => set({ user }),
	clearUser: () => set({ user: null, isLoading: false }),
}));

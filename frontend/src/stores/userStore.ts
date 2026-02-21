import {create} from 'zustand'
import type AppUser from "../domain/models/appUser.ts";

interface UserState {
    user: AppUser | null;
    setUser: (user: AppUser) => void;
    clearUser: () => void;
}

export const useUserStore = create<UserState>((set) => ({
    user: null,
    setUser: (user) => set({user}),
    clearUser: () => set({user: null}),
}));
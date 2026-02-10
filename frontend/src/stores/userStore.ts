import type {UserResponse} from "../api/openApi/model";
import {create} from 'zustand'

interface UserState {
    user: UserResponse | null;
    setUser: (user: UserResponse) => void;
    clearUser: () => void;
}

export const useUserStore = create<UserState>((set) => ({
    user: null,
    setUser: (user) => set({user}),
    clearUser: () => set({user: null}),
}));
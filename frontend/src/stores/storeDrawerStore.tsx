import { create } from 'zustand';

interface StoreDrawerState {
	isOpen: boolean;
	open: () => void;
	close: () => void;
}

export const useStoreDrawerStore = create<StoreDrawerState>(set => ({
	isOpen: true,
	open: () => set({ isOpen: true }),
	close: () => set({ isOpen: false }),
}));

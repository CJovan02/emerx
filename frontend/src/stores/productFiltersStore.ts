import { create } from 'zustand';

interface ProductFiltersState {
	search: string;
	category: string | null;
	minPrice: number | null;
	maxPrice: number | null;
	minRating: number | null;
	inStockOnly: boolean;
	setSearch: (search: string) => void;
	setCategory: (category: string | null) => void;
	setMinPrice: (value: number | null) => void;
	setMaxPrice: (value: number | null) => void;
	setMinRating: (rating: number | null) => void;
	setInStockOnly: (value: boolean) => void;
	clearFilters: () => void;
}

const defaultFilters = {
	search: '',
	category: null,
	minPrice: null,
	maxPrice: null,
	minRating: null,
	inStockOnly: false,
};

export const useProductFiltersStore = create<ProductFiltersState>(set => ({
	...defaultFilters,
	setSearch: search => set({ search }),
	setCategory: category => set({ category }),
	setMinPrice: minPrice => set({ minPrice }),
	setMaxPrice: maxPrice => set({ maxPrice }),
	setMinRating: minRating => set({ minRating }),
	setInStockOnly: inStockOnly => set({ inStockOnly }),
	clearFilters: () => set(defaultFilters),
}));

export function hasActiveFilters(state: ProductFiltersState): boolean {
	return (
		state.search !== '' ||
		state.category !== null ||
		state.minPrice !== null ||
		state.maxPrice !== null ||
		state.minRating !== null ||
		state.inStockOnly
	);
}

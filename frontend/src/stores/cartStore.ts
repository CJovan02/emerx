import { persist } from 'zustand/middleware';
import { create } from 'zustand';
import type { CartItem } from '../domain/models/cartItem.ts';

interface CartStore {
	items: CartItem[];
	addToCart: (item: CartItem) => void;
	removeFromCart: (productId: string) => void;
	clearCart: () => void;
}

export const useCartStore = create<CartStore>()(
	persist(
		set => ({
			items: [],

			addToCart: item => set(state => handleAddToCart(state, item)),

			removeFromCart: id => set(state => handleRemoveFromCart(state, id)),

			clearCart: () =>
				set({
					items: [],
				}),
		}),
		{
			name: 'anonymous-cart-storage',
			// in order to generate local storage key like this: '{user-id}-cart-storage'
			// we need to manually set the persistName and manually hydrate the store when auth state changes
			// I hydrate this in /src/components/authFlow/authUserSync.tsx
			// skipHydration: true,
		}
	)
);

// Handlers
function handleAddToCart(state: CartStore, item: CartItem): Partial<CartStore> {
	// // only allow cart to have 10 unique items
	// if (state.items.length >= 10) return state;
	if (item.quantity < 1) return state;

	const existing = state.items.find(i => i.product.id === item.product.id);

	// if this product exists in store, we sum the quantity of both
	if (existing) {
		const newQuantity = existing.quantity + item.quantity;
		const newProduct = { ...existing, quantity: newQuantity };
		return {
			items: state.items.map(i =>
				i.product.id === item.product.id ? newProduct : i
			),
		};
	}

	return { items: [...state.items, item] };
}

function handleRemoveFromCart(state: CartStore, productId: string) {
	return {
		items: state.items.filter(i => i.product.id !== productId),
	};
}

// utils
export function getQuantityForProduct(
	items: CartItem[],
	productId: string
): number | undefined {
	return items.find(i => i.product.id === productId)?.quantity;
}

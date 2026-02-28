import { useCartStore } from '../../stores/cartStore.ts';

export default function useCartLogic() {
	const cartItems = useCartStore(state => state.items);
	const addToCart = useCartStore(state => state.addToCart);
	const removeFromCart = useCartStore(state => state.removeFromCart);
	const clearCart = useCartStore(state => state.clearCart);

	return {
		cartItems,
		addToCart,
		removeFromCart,
		clearCart,
	};
}

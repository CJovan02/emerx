import type { ProductResponse } from '../../api/openApi/model';

export type CartItem = {
	product: ProductResponse;
	quantity: number;
};

export function getPriceForCartItem(item: CartItem): number {
	return item.quantity * item.quantity;
}

export function getCartTotalPrice(cart: CartItem[]): number {
	return cart.reduce((sum, item) => sum + getPriceForCartItem(item), 0);
}

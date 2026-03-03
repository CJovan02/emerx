import type {
	OrderReviewItem,
	OrderReviewResponse,
} from '../../api/openApi/model';

export type CartItem = {
	productId: string;
	name: string;
	thumbnailUrl?: string | null;
	unitPrice: number;
	stock: number;
	quantity: number;
};

export function getPriceForCartItem(item: CartItem): number {
	return item.quantity * item.unitPrice;
}

export function getCartTotalPrice(cart: CartItem[]): number {
	return cart.reduce((sum, item) => sum + getPriceForCartItem(item), 0);
}

export function mapOrderReviewItemToDomain(item: OrderReviewItem): CartItem {
	return {
		productId: item.productId,
		unitPrice: item.unitPrice,
		name: item.productName,
		thumbnailUrl: item.imageUrl,
		stock: item.stock,
		quantity: item.quantity,
	} as CartItem;
}

export function mapOrderReviewResponseToDomain(
	response: OrderReviewResponse
): CartItem[] {
	return response.items.map(mapOrderReviewItemToDomain);
}

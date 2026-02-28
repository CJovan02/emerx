import { useProductGetById } from '../../api/openApi/product/product.ts';
import { useCallback, useMemo } from 'react';
import { isAxiosError } from 'axios';
import { getQuantityForProduct, useCartStore } from '../../stores/cartStore.ts';
import type { CartItem } from '../../domain/models/cartItem.ts';

type AddToCartResult = {
	success: boolean;
	errorMessage?: string;
	errorType?: 'not-enough-stock' | 'general';
};

type ErrorStatus = {
	type: 'general' | 'product-not-found';
	message: string;
};

function useProductDetailsLogic(productId: string) {
	const cartItems = useCartStore(state => state.items);
	// this is used for validation of adding to cart
	const productQuantityInCart =
		getQuantityForProduct(cartItems, productId) ?? 0;
	const addToCartStore = useCartStore(state => state.addToCart);

	// react query
	const query = useProductGetById(productId);
	const { isError, error, data, refetch, isRefetching, isPending } = query;

	const queryErrorStatus: ErrorStatus | null = useMemo(() => {
		if (!isError) {
			return null;
		}

		console.error(error);

		if (isAxiosError(error)) {
			switch (error.response?.status) {
				case 404:
					return {
						type: 'product-not-found',
						message: 'Product not found',
					};
			}
		}

		return {
			type: 'general',
			message: 'Unexpected error happened, please try again.',
		};
	}, [isError, error]);

	// business logic
	const addToCart = useCallback(
		(quantity: number): AddToCartResult => {
			if (quantity < 1)
				return {
					success: false,
					errorType: 'general',
					errorMessage: "Quantity can't be lass than 1",
				};

			if (!data)
				return {
					success: false,
					errorType: 'general',
					errorMessage: "You can't add to cart if product is not loaded",
				};

			// If new quantity that user selected combined with the quantity that he has in cart is bigger than the stock
			// of the product, than he can't add to cart that many products
			const quantitySum = quantity + productQuantityInCart;
			if (quantitySum > data.stock) {
				let mess = 'That quantity is not available.';
				if (productQuantityInCart > 0) {
					mess = `${mess} You already have ${productQuantityInCart} items in cart for this product.`;
				}

				return {
					success: false,
					errorMessage: mess,
					errorType: 'not-enough-stock',
				};
			}

			const item: CartItem = {
				product: data,
				quantity,
			};

			addToCartStore(item);
			return { success: true };
		},
		[data, productQuantityInCart, addToCartStore]
	);

	return {
		product: data,
		isError,
		refetch,
		isPending,
		isRefetching,
		productQuantityInCart,
		queryErrorStatus,
		addToCart,
	};
}

export default useProductDetailsLogic;

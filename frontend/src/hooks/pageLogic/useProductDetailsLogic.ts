import { useProductGetById } from '../../api/openApi/product/product.ts';
import { useCallback, useEffect, useState } from 'react';
import { isAxiosError } from 'axios';
import { useCartStore } from '../../stores/cartStore.ts';
import type { CartItem } from '../../domain/models/cartItem.ts';

function useProductDetailsLogic(productId: string) {
	const [errorMessage, setErrorMessage] = useState<string | null>(null);
	const [productNotFound, setProductNotFound] = useState(false);

	const addToCartStore = useCartStore(state => state.addToCart);

	// react query
	const query = useProductGetById(productId);
	const { isError, error } = query;

	// business logic
	const addToCart = useCallback(
		(quantity: number) => {
			if (quantity < 1) throw Error('Quantity must be greater than 1');
			if (isError || !query.data) return;

			const item: CartItem = {
				product: query.data,
				quantity,
			};
			addToCartStore(item);
		},
		[addToCartStore]
	);

	useEffect(() => {
		if (!isError) {
			setErrorMessage(null);
			return;
		}

		console.error(error);

		if (isAxiosError(error)) {
			switch (error.response?.status) {
				case 404:
					setErrorMessage('Product not found');
					setProductNotFound(true);
					return;
			}
		}

		setErrorMessage('Unexpected error happened, please try again.');
	}, [isError, error, setErrorMessage, setProductNotFound]);

	return {
		...query,
		errorMessage,
		productNotFound,
		addToCart,
	};
}

export default useProductDetailsLogic;

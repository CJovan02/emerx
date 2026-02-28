import { useProductGetById } from '../../api/openApi/product/product.ts';
import { useCallback, useEffect, useState } from 'react';
import { isAxiosError } from 'axios';
import { getQuantityForProduct, useCartStore } from '../../stores/cartStore.ts';
import type { CartItem } from '../../domain/models/cartItem.ts';

function useProductDetailsLogic(productId: string) {
	const cartItems = useCartStore(state => state.items);
	const addToCartStore = useCartStore(state => state.addToCart);

	const [errorMessage, setErrorMessage] = useState<string | null>(null);
	const [productNotFound, setProductNotFound] = useState(false);
	// this is used to signal UI layer that the item is added to cart successfully
	const [itemAddedSignal, setItemAddedSignal] = useState<number>(0);
	const [quantityNotAvailableMessage, setQuantityNotAvailableMessage] =
		useState<string | null>(null);
	// this is used for validation of adding to cart
	const [productQuantityInCart, setProductQuantityInCart] = useState<number>(0);

	// react query
	const query = useProductGetById(productId);
	const { isError, error, data } = query;

	// business logic
	useEffect(() => {
		const q = getQuantityForProduct(cartItems, productId) ?? 0;
		setProductQuantityInCart(q);
	}, [cartItems, productId, setProductQuantityInCart]);

	const addToCart = useCallback(
		(quantity: number) => {
			if (quantity < 1) throw Error('Quantity must be greater than 1');
			if (isError || !data) return;

			// If new quantity that user selected combined with the quantity that he has in cart is bigger than the stock
			// of the product, than he can't add to cart that many products
			const quantitySum = quantity + productQuantityInCart;
			if (quantitySum > data.stock) {
				let mess = 'That quantity is not available.';
				if (productQuantityInCart > 0) {
					mess = `${mess} You already have ${productQuantityInCart} items in cart for this product.`;
				}

				setQuantityNotAvailableMessage(mess);
				return;
			}
			setQuantityNotAvailableMessage(null);

			const item: CartItem = {
				product: data,
				quantity,
			};

			addToCartStore(item);
			setItemAddedSignal(prev => prev + 1);
		},
		[
			isError,
			data,
			productQuantityInCart,
			setQuantityNotAvailableMessage,
			addToCartStore,
			setItemAddedSignal,
		]
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
		quantityNotAvailableMessage,
		clearQuantityMessage: () => setQuantityNotAvailableMessage(null),
		itemAddedSignal,
		errorMessage,
		productNotFound,
		productQuantityInCart,
		addToCart,
	};
}

export default useProductDetailsLogic;

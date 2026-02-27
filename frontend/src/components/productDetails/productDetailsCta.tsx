import { Box, Button } from '@mui/material';
import { AddShoppingCart } from '@mui/icons-material';
import InputStepper from '../../shared/components/ui/inputStepper.tsx';
import { useState } from 'react';

type Props = {
	stock: number;
	onAddToCart: (quantity: number) => void;
};

export default function ProductDetailsCta({ stock, onAddToCart }: Props) {
	const [quantity, setQuantity] = useState(1);
	const inStock = stock > 0;
	const isMaxQuantity = quantity >= stock;

	function addToCart() {
		if (quantity > stock) return;
		onAddToCart(quantity);
	}

	function increment() {
		if (isMaxQuantity) return;
		setQuantity(prev => prev + 1);
	}

	function decrement() {
		setQuantity(prev => (prev > 1 ? prev - 1 : prev));
	}

	return (
		<Box
			display='flex'
			justifyContent='center'
			gap={3}>
			<InputStepper
				isMaximum={isMaxQuantity}
				title='Item Quantity'
				value={quantity}
				disabled={!inStock}
				onIncrement={increment}
				onDecrement={decrement}
			/>

			<Button
				startIcon={<AddShoppingCart />}
				variant='contained'
				size='large'
				onClick={addToCart}
				disabled={!inStock}
				sx={{ flex: 1, fontWeight: 'bold', fontSize: 17 }}>
				Add to Cart
			</Button>
		</Box>
	);
}

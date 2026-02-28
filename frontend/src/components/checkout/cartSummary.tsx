import { Box, Button, Typography } from '@mui/material';
import {
	type CartItem,
	getCartTotalPrice,
} from '../../domain/models/cartItem.ts';
import { formatCurrency } from '../../utils/utils.ts';
import CheckoutCartItemCard from './checkoutCartItemCard.tsx';

type Props = {
	items: CartItem[];
};

export default function CartSummary({ items }: Props) {
	const total = formatCurrency(getCartTotalPrice(items));

	return (
		<Box>
			<Typography
				variant='h6'
				px={3}
				pt={3}
				mb={2}>
				Cart Summary
			</Typography>

			{items.map(item => (
				<CheckoutCartItemCard
					key={item.product.id}
					item={item}
				/>
			))}

			<Typography
				mt={6}
				px={3}
				pb={3}
				variant='h6'>
				Total: {total}
			</Typography>

			<Box
				px={3}
				pb={3}>
				<Button
					sx={{
						height: 55,
						fontSize: 15,
						fontWeight: 700
					}}
					fullWidth>
					Continue to Review
				</Button>
			</Box>
		</Box>
	);
}

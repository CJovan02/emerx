import { Box, Button, Typography } from '@mui/material';
import {
	type CartItem,
	getCartTotalPrice,
} from '../../domain/models/cartItem.ts';
import { formatCurrency } from '../../utils/utils.ts';
import CheckoutOrderItem from './checkoutOrderItem.tsx';
import { ArrowForwardOutlined } from '@mui/icons-material';

type Props = {
	items: CartItem[];
};

export default function CartSummary({ items }: Props) {
	const total = formatCurrency(getCartTotalPrice(items));

	return (
		<Box>
			<Typography
				variant='h5'
				fontWeight={600}
				px={3}
				pt={3}
				mb={2}>
				Review your cart
			</Typography>

			{items.map(item => (
				<CheckoutOrderItem
					key={item.productId}
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
					startIcon={<ArrowForwardOutlined />}
					form='checkout-form'
					type='submit'
					sx={{
						height: 55,
						fontSize: 15,
						fontWeight: 700,
					}}
					fullWidth>
					Continue to Review
				</Button>
			</Box>
		</Box>
	);
}

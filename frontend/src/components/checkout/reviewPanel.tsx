import { Box, Button, Typography, Divider } from '@mui/material';
import {
	type CartItem,
	getCartTotalPrice,
} from '../../domain/models/cartItem.ts';
import type { AddressRequiredDto } from '../../api/openApi/model';

type Props = {
	items: CartItem[];
	address: AddressRequiredDto;
	onBack: () => void;
	onConfirm: () => void;
	loading?: boolean;
};

export default function ReviewPanel({
	items,
	address,
	onBack,
	onConfirm,
	loading,
}: Props) {
	const total = getCartTotalPrice(items);

	return (
		<Box>
			<Typography
				variant='h6'
				mb={2}>
				Review Order
			</Typography>

			<Typography
				fontWeight={500}
				mb={1}>
				Shipping Address
			</Typography>
			<Typography variant='body2'>{address.city}</Typography>
			<Typography variant='body2'>{address.street}</Typography>
			<Typography
				variant='body2'
				mb={2}>
				{address.houseNumber}
			</Typography>

			<Divider sx={{ my: 2 }} />

			{items.map(item => (
				<Box
					key={item.product.id}
					mb={1}>
					<Typography>
						{item.product.name} ({item.quantity} × €{item.product.price})
					</Typography>
				</Box>
			))}

			<Typography
				mt={2}
				fontWeight={600}>
				Total: €{total}
			</Typography>

			<Box
				display='flex'
				gap={2}
				mt={3}>
				<Button
					fullWidth
					variant='outlined'
					onClick={onBack}>
					Back
				</Button>

				<Button
					fullWidth
					variant='contained'
					onClick={onConfirm}
					disabled={loading}>
					Confirm Order
				</Button>
			</Box>
		</Box>
	);
}

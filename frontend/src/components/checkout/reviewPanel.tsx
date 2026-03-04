import { Box, Button, Typography, Divider } from '@mui/material';
import {
	type CartItem,
	getCartTotalPrice,
} from '../../domain/models/cartItem.ts';
import type { AddressRequiredDto } from '../../api/openApi/model';
import AddressFieldInfo from './addressFIeldInfo.tsx';
import {
	ArrowBackOutlined,
	CheckCircleOutline,
	HomeOutlined,
	LocationCityOutlined,
} from '@mui/icons-material';
import CheckoutOrderItem from './checkoutOrderItem.tsx';
import { formatCurrency } from '../../utils/utils.ts';

type Props = {
	items: CartItem[];
	address: AddressRequiredDto;
	onBack: () => void;
	onConfirm: () => void;
};

export default function ReviewPanel({
	items,
	address,
	onBack,
	onConfirm,
}: Props) {
	const total = formatCurrency(getCartTotalPrice(items));

	return (
		<Box>
			<Typography
				fontWeight={600}
				variant='h5'
				mb={2}>
				Shipping Address
			</Typography>

			<AddressFieldInfo
				value={address.city}
				icon={<LocationCityOutlined />}
			/>
			<AddressFieldInfo
				value={address.street + ' - ' + address.houseNumber}
				icon={<HomeOutlined />}
			/>

			<Divider sx={{ my: 2 }} />

			{items.map(item => (
				<CheckoutOrderItem
					key={item.productId}
					item={item}
					disableClick
					disableGutters
				/>
			))}

			<Typography
				mt={4}
				variant='h6'
				fontWeight={600}>
				Total: {total}
			</Typography>

			<Box
				display='flex'
				gap={2}
				mt={3}>
				<Button
					startIcon={<ArrowBackOutlined />}
					fullWidth
					variant='outlined'
					onClick={onBack}
					sx={{
						height: 55,
					}}>
					Back
				</Button>

				<Button
					startIcon={<CheckCircleOutline />}
					fullWidth
					variant='contained'
					onClick={onConfirm}
					sx={{
						height: 55,
						fontSize: 15,
						fontWeight: 700,
					}}>
					Confirm Order
				</Button>
			</Box>
		</Box>
	);
}

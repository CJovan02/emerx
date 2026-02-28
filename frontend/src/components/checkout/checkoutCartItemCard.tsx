import { Box, Typography } from '@mui/material';
import {
	type CartItem,
	getPriceForCartItem,
} from '../../domain/models/cartItem.ts';
import ImagePlaceholder from '../../shared/components/ui/imagePlaceholder.tsx';
import { formatCurrency } from '../../utils/utils.ts';
import { useNavigate } from 'react-router';
import { Routes } from '../../shared/common/constants/routeNames.ts';

export default function CheckoutCartItemCard({ item }: { item: CartItem }) {
	const navigate = useNavigate();
	const navigateToProductDetails = () =>
		navigate(Routes.ProductDetails(item.product.id));

	return (
		<Box
			onClick={navigateToProductDetails}
			display='flex'
			alignItems='center'
			gap={2}
			py={2}
			px={3}
			sx={{
				cursor: 'pointer',
				'&:hover': {
					backgroundColor: 'action.hover',
				},
			}}>
			{/* Thumbnail */}
			{item.product.thumbnailUrl && (
				<Box
					component='img'
					src={item.product.thumbnailUrl}
					alt={item.product.name}
					sx={{
						aspectRatio: '1/1',
						width: 86,
						height: 86,
						boxShadow: 1,
						objectFit: 'cover',
						borderRadius: 0.65,
					}}
				/>
			)}
			{!item.product.thumbnailUrl && (
				<ImagePlaceholder
					height={86}
					width={86}
					borderRadius={0.65}
					fontSize={28}
				/>
			)}

			{/* Info */}
			<Box flexGrow={1}>
				<Typography
					fontWeight={500}
					noWrap>
					{item.product.name}
				</Typography>
				<Typography
					variant='body2'
					color='text.secondary'
					fontWeight={300}>
					{formatCurrency(item.product.price)} × {item.quantity}
				</Typography>
				<Typography
					mt={1}
					fontWeight={600}>
					{formatCurrency(getPriceForCartItem(item))}
				</Typography>
			</Box>
		</Box>
	);
}

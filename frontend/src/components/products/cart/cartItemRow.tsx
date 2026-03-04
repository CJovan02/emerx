import { Box, Typography, IconButton } from '@mui/material';
import CloseIcon from '@mui/icons-material/Close';
import type { CartItem } from '../../../domain/models/cartItem.ts';
import { formatCurrency } from '../../../utils/utils.ts';
import ImagePlaceholder from '../../../shared/components/ui/imagePlaceholder.tsx';

interface CartItemRowProps {
	item: CartItem;
	onRemove: (productId: string) => void;
	onClick?: (productId: string) => void;
}

export default function CartItemRow({
	item,
	onRemove,
	onClick,
}: CartItemRowProps) {
	const { quantity } = item;

	const totalPrice = quantity * item.unitPrice;

	return (
		<Box
			py={1.5}
			sx={{
				cursor: onClick ? 'pointer' : 'default',
				'&:hover': {
					backgroundColor: 'action.hover',
				},
			}}
			onClick={() => onClick?.(item.productId)}>
			<Box
				display='flex'
				alignItems='center'
				gap={2}
				px={2}>
				{/* Thumbnail */}
				{item.thumbnailUrl && (
					<Box
						component='img'
						src={item.thumbnailUrl}
						alt={item.name}
						sx={{
							width: 64,
							height: 64,
							boxShadow: 1,
							objectFit: 'cover',
							borderRadius: 0.5,
							flexShrink: 0,
						}}
					/>
				)}
				{!item.thumbnailUrl && (
					<ImagePlaceholder
						width={64}
						height={64}
						borderRadius={0.5}
					/>
				)}

				{/* Info */}
				<Box
					flexGrow={1}
					minWidth={0}>
					<Typography
						variant='body2'
						fontWeight={500}
						noWrap>
						{item.name}
					</Typography>

					<Typography
						variant='body2'
						color='text.secondary'
						fontWeight={300}>
						{formatCurrency(item.unitPrice)} × {quantity}
					</Typography>

					<Typography
						mt={0.5}
						variant='body2'
						fontWeight={600}>
						{formatCurrency(totalPrice)}
					</Typography>
				</Box>

				{/* Remove */}
				<IconButton
					size='small'
					onClick={e => {
						e.stopPropagation();
						onRemove(item.productId);
					}}>
					<CloseIcon fontSize='small' />
				</IconButton>
			</Box>
		</Box>
	);
}

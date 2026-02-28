import { Box, Typography, IconButton } from '@mui/material';
import CloseIcon from '@mui/icons-material/Close';
import type { CartItem } from '../../../domain/models/cartItem.ts';
import {formatCurrency} from "../../../utils/utils.ts";
import ImagePlaceholder from "../../../shared/components/ui/imagePlaceholder.tsx";

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
	const { product, quantity } = item;

	const totalPrice = quantity * product.price;

	return (
		<Box
			py={1.5}
			sx={{
				cursor: onClick ? 'pointer' : 'default',
				'&:hover': {
					backgroundColor: 'action.hover',
				},
			}}
			onClick={() => onClick?.(product.id)}>
			<Box
                display='flex'
                alignItems='center'
                gap={2}
                px={2}
            >
				{/* Thumbnail */}
				{product.thumbnailUrl && (
					<Box
						component='img'
						src={product.thumbnailUrl}
						alt={product.name}
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
				{!product.thumbnailUrl && (
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
						{product.name}
					</Typography>

					<Typography
						variant='body2'
						color='text.secondary'
                        fontWeight={300}
                    >
						{formatCurrency(product.price)} × {quantity}
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
						onRemove(product.id);
					}}>
					<CloseIcon fontSize='small' />
				</IconButton>
			</Box>
		</Box>
	);
}

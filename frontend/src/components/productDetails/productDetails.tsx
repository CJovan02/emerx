import { Box, Grid, Typography, Chip, Rating, Divider } from '@mui/material';
import type { ProductResponse } from '../../api/openApi/model';
import { formatCurrency } from '../../utils/utils.ts';
import ProductStock from './productStock.tsx';
import ProductDetailImage from './productDetailsImage.tsx';
import ProductDetailsCta from './productDetailsCta.tsx';

type Props = {
	product: ProductResponse;
	onAddToCart: (quantity: number) => void;
};

export default function ProductDetails({ product, onAddToCart }: Props) {
	const {
		thumbnailUrl,
		name,
		reviewCount,
		description,
		stock,
		category,
		averageRating,
		price,
	} = product;

	return (
		<Box
			maxWidth='lg'
			mx='auto'
			p={4}>
			<Grid
				container
				spacing={6}>
				{/* LEFT COLUMN - IMAGE */}
				<Grid
					size={{
						xs: 12,
						md: 6,
					}}>
					<ProductDetailImage
						thumbnailUrl={thumbnailUrl}
						name={name}
					/>
				</Grid>

				{/* RIGHT COLUMN - INFO */}
				<Grid
					size={{
						xs: 12,
						md: 6,
					}}>
					<Typography
						variant='h4'
						fontWeight={600}
						gutterBottom>
						{name}
					</Typography>

					<Chip
						label={category}
						sx={{ mb: 2 }}
					/>

					<Typography
						variant='body1'
						color='text.secondary'
						fontWeight={300}
						mb={4}
						sx={{
							whiteSpace: 'pre-wrap',
							wordBreak: 'break-word',
						}}>
						{description}
					</Typography>

					<Box
						display='flex'
						alignItems='center'
						gap={1}
						mb={2}>
						<Rating
							value={averageRating}
							precision={0.1}
							size='large'
							readOnly
						/>
						<Typography variant='body2'>
							{reviewCount} Customer Reviews
						</Typography>
					</Box>

					<Typography
						variant='h5'
						fontWeight={700}
						mb={1}>
						{formatCurrency(price)}
					</Typography>

					<ProductStock stock={stock} />

					<Divider sx={{ my: 3 }} />

					<ProductDetailsCta
						stock={stock}
						onAddToCart={onAddToCart}
					/>
				</Grid>
			</Grid>
		</Box>
	);
}

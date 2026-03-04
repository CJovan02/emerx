import {
	Box,
	Card,
	CardActionArea,
	CardContent,
	CardMedia,
	Chip,
	Rating,
	Typography,
} from '@mui/material';
import ImageNotSupportedOutlinedIcon from '@mui/icons-material/ImageNotSupportedOutlined';
import type { ProductResponse } from '../../api/openApi/model';
import { useNavigate } from 'react-router';
import { Routes } from '../../shared/common/constants/routeNames.ts';
import ProductStock from '../productDetails/productStock.tsx';
import { formatCurrency } from '../../utils/utils.ts';

type Props = {
	product: ProductResponse;
};

export default function ProductCard({ product }: Props) {
	const navigate = useNavigate();

	const handleClick = () => {
		void navigate(Routes.ProductDetails(product.id));
	};

	return (
		<Card sx={{ height: '100%', display: 'flex', flexDirection: 'column' }}>
			<CardActionArea
				onClick={handleClick}
				sx={{
					flexGrow: 1,
					display: 'flex',
					flexDirection: 'column',
					alignItems: 'stretch',
				}}>
				{product.thumbnailUrl ? (
					<CardMedia
						component='img'
						image={product.thumbnailUrl}
						alt={product.name}
						sx={{ height: 180, objectFit: 'cover' }}
					/>
				) : (
					<Box
						sx={{
							height: 180,
							display: 'flex',
							alignItems: 'center',
							justifyContent: 'center',
							bgcolor: 'surface.muted',
						}}>
						<ImageNotSupportedOutlinedIcon
							sx={{ fontSize: 48, color: 'text.disabled' }}
						/>
					</Box>
				)}

				<CardContent
					sx={{
						flexGrow: 1,
						display: 'flex',
						flexDirection: 'column',
						gap: 0.5,
					}}>
					<Chip
						label={product.category}
						size='small'
						color='primary'
						variant='outlined'
						sx={{ alignSelf: 'flex-start', mb: 0.5 }}
					/>

					<Typography
						variant='subtitle1'
						fontWeight={600}
						sx={{
							overflow: 'hidden',
							display: '-webkit-box',
							WebkitLineClamp: 2,
							WebkitBoxOrient: 'vertical',
						}}>
						{product.name}
					</Typography>

					<Box
						sx={{
							display: 'flex',
							alignItems: 'center',
							gap: 0.5,
							mt: 'auto',
						}}>
						<Rating
							value={product.averageRating}
							precision={0.5}
							size='small'
							readOnly
						/>
						<Typography
							variant='caption'
							fontWeight={300}
							color='text.secondary'>
							({product.reviewCount})
						</Typography>
					</Box>

					<Box
						sx={{
							display: 'flex',
							alignItems: 'center',
							justifyContent: 'space-between',
							mt: 1,
						}}>
						<Typography
							variant='h6'
							fontWeight={700}
							color='primary'>
							{formatCurrency(product.price)}
						</Typography>
						<ProductStock
							stock={product.stock}
							size='small'
						/>
					</Box>
				</CardContent>
			</CardActionArea>
		</Card>
	);
}

import {
	Box,
	Button,
	Grid,
	Pagination,
	Skeleton,
	Typography,
} from '@mui/material';
import useProductsPageLogic from '../hooks/pageLogic/useProductsPageLogic.ts';
import ProductCard from '../components/products/ProductCard.tsx';

const SKELETON_COUNT = 8;

export default function ProductsPage() {
	const { products, isLoading, isError, refetch, page, setPage, totalPages } =
		useProductsPageLogic();

	if (isError) {
		return (
			<Box
				sx={{
					display: 'flex',
					flexDirection: 'column',
					alignItems: 'center',
					gap: 2,
					mt: 8,
				}}>
				<Typography color='error'>Failed to load products.</Typography>
				<Button onClick={() => void refetch()}>Retry</Button>
			</Box>
		);
	}

	return (
		<Box sx={{ display: 'flex', flexDirection: 'column', gap: 3 }}>
			<Grid container spacing={2}>
				{isLoading
					? Array.from({ length: SKELETON_COUNT }).map((_, i) => (
							<Grid key={i} size={{ xs: 12, sm: 6, md: 4, lg: 3 }}>
								<Skeleton variant='rounded' height={320} />
							</Grid>
						))
					: products.map(product => (
							<Grid key={product.id} size={{ xs: 12, sm: 6, md: 4, lg: 3 }}>
								<ProductCard product={product} />
							</Grid>
						))}
			</Grid>

			{!isLoading && products.length === 0 && (
				<Typography color='text.secondary' textAlign='center' sx={{ mt: 8 }}>
					No products found.
				</Typography>
			)}

			{totalPages > 1 && (
				<Box sx={{ display: 'flex', justifyContent: 'center' }}>
					<Pagination
						count={totalPages}
						page={page}
						onChange={(_, value) => setPage(value)}
						color='primary'
					/>
				</Box>
			)}
		</Box>
	);
}

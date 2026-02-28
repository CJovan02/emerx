import { Navigate, useNavigate, useParams } from 'react-router';
import { Routes } from '../shared/common/constants/routeNames.ts';
import useProductDetailsLogic from '../hooks/pageLogic/useProductDetailsLogic.ts';
import { Alert, Box, Button, Container } from '@mui/material';
import { ArrowBack, Refresh } from '@mui/icons-material';
import CircularProgress from '@mui/material/CircularProgress';
import ProductDetails from '../components/productDetails/productDetails.tsx';
import { useSnackbar } from 'notistack';

export function ProductDetailsPage() {
	const { id } = useParams();
	const navigate = useNavigate();
	const navigateToProducts = () => navigate(Routes.Products, { replace: true });
	if (!id)
		return (
			<Navigate
				to={Routes.Products}
				replace
			/>
		);

	const {
		isError,
		refetch,
		isRefetching,
		isPending,
		product,
		addToCart,
		queryErrorStatus,
	} = useProductDetailsLogic(id);
	const { enqueueSnackbar } = useSnackbar();

	function handleAddToCart(quantity: number) {
		const result = addToCart(quantity);
		if (result.success) {
			enqueueSnackbar('Successfully added to cart.', { variant: 'success' });
			return;
		}

		if (result.errorType === 'general') {
			enqueueSnackbar(result.errorMessage!, { variant: 'error' });
			return;
		}

		if (result.errorType === 'not-enough-stock') {
			enqueueSnackbar(result.errorMessage!, {
				variant: 'warning',
				autoHideDuration: 7000,
			});
			return;
		}
	}

	if (isPending) {
		return (
			<Box
				width='100%'
				display='flex'
				justifyContent='center'>
				<CircularProgress size={45} />
			</Box>
		);
	}

	if (isError) {
		return (
			<Container maxWidth='xs'>
				<Alert severity='error'>{queryErrorStatus!.message}</Alert>
				<Box
					mt={3}
					display='flex'
					gap={3}
					width='100%'>
					<Button
						startIcon={<ArrowBack />}
						onClick={navigateToProducts}
						variant='outlined'
						sx={{
							height: 45,
							fontWeight: 700,
							flex: 1,
						}}>
						Go to products
					</Button>

					{queryErrorStatus!.type !== 'product-not-found' && (
						<Button
							startIcon={<Refresh />}
							onClick={_ => refetch()}
							loading={isRefetching}
							sx={{
								height: 45,
								fontWeight: 700,
								flex: 1,
							}}>
							Refresh
						</Button>
					)}
				</Box>
			</Container>
		);
	}

	return (
		<ProductDetails
			product={product!}
			onAddToCart={handleAddToCart}
		/>
	);
}

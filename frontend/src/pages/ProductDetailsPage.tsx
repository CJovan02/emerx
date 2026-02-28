import { Navigate, useNavigate, useParams } from 'react-router';
import { Routes } from '../shared/common/constants/routeNames.ts';
import useProductDetailsLogic from '../hooks/pageLogic/useProductDetailsLogic.ts';
import { Alert, Box, Button, Container } from '@mui/material';
import { ArrowBack, Refresh } from '@mui/icons-material';
import CircularProgress from '@mui/material/CircularProgress';
import ProductDetails from '../components/productDetails/productDetails.tsx';
import { useEffect } from 'react';
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
		errorMessage,
		isError,
		quantityNotAvailableMessage,
		clearQuantityMessage,
		refetch,
		isRefetching,
		productNotFound,
		isPending,
		data,
		addToCart,
		itemAddedSignal
	} = useProductDetailsLogic(id);
	const { enqueueSnackbar } = useSnackbar();

	useEffect(() => {
		if (!quantityNotAvailableMessage) return;

		enqueueSnackbar(quantityNotAvailableMessage, { variant: 'warning', autoHideDuration: 8000 });
		clearQuantityMessage();
	}, [enqueueSnackbar, quantityNotAvailableMessage, clearQuantityMessage]);

	useEffect(() => {
		enqueueSnackbar("Successfully added item to cart", {variant: 'success'});
	}, [itemAddedSignal]);


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
				<Alert severity='error'>{errorMessage}</Alert>
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

					{!productNotFound && (
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
			product={data!}
			onAddToCart={addToCart}
		/>
	);
}

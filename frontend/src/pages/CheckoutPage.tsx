import {
	Alert,
	Box,
	Button,
	Container,
	Paper,
	Typography,
	Backdrop,
} from '@mui/material';
import CartSummary from '../components/checkout/cartSummary.tsx';
import AddressForm from '../components/checkout/addressForm.tsx';
import ReviewPanel from '../components/checkout/reviewPanel.tsx';
import useCheckoutLogic from '../hooks/pageLogic/useCheckoutLogic.ts';
import type { AddressRequiredDto } from '../api/openApi/model';
import CircularProgress from '@mui/material/CircularProgress';
import { mapOrderReviewResponseToDomain } from '../domain/models/cartItem.ts';
import { Refresh } from '@mui/icons-material';
import { useEffect } from 'react';
import { useSnackbar } from 'notistack';
import { useNavigate } from 'react-router';
import { Routes } from '../shared/common/constants/routeNames.ts';

export default function CheckoutPage() {
	const {
		cartItems,
		handleFormContinueToReview,
		form,
		goBackToForm,
		isFormStage,
		isReviewStage,
		reviewData,
		reviewIsError,
		reviewErrorMessage,
		reviewIsPending,
		createIsPending,
		createIsSuccess,
		createIsError,
		createErrorMessage,
		handleSubmitOrder,
	} = useCheckoutLogic();
	const { enqueueSnackbar } = useSnackbar();
	const navigate = useNavigate();

	const address: AddressRequiredDto = {
		houseNumber: form.watch('houseNumber'),
		city: form.watch('city'),
		street: form.watch('street'),
	};

	useEffect(() => {
		if (!createIsSuccess) return;

		enqueueSnackbar('Successfully placed an order!', { variant: 'success' });
		navigate(Routes.Products);
	}, [createIsSuccess]);

	useEffect(() => {
		if (!createIsError) return;

		enqueueSnackbar(createErrorMessage, { variant: 'error' });
	}, [createErrorMessage, createIsError]);

	return (
		<>
			<Backdrop
				open={createIsPending}
				onClick={() => {}}
				sx={theme => ({
					zIndex: theme.zIndex.drawer + 1,
				})}>
				<CircularProgress
					size={50}
					sx={theme => ({ color: theme.palette.primary.contrastText })}
				/>
			</Backdrop>

			<Container
				maxWidth='lg'
				sx={{ pt: 6 }}>
				<Typography
					variant='h4'
					fontWeight={600}
					mb={4}>
					{isFormStage && 'Checkout'}
					{isReviewStage && 'Review Your Order'}
				</Typography>

				{isFormStage && (
					<Box
						display='flex'
						gap={4}
						alignItems='flex-start'>
						<Paper sx={{ flex: 1, p: 3 }}>
							<AddressForm
								onSubmit={handleFormContinueToReview}
								form={form}
							/>
						</Paper>

						<Paper sx={{ flex: 1 }}>
							<CartSummary items={cartItems} />
						</Paper>
					</Box>
				)}

				{isReviewStage && reviewIsError && (
					<Container maxWidth='xs'>
						<Alert severity='error'>{reviewErrorMessage}</Alert>
						<Button
							startIcon={<Refresh />}
							onClick={handleFormContinueToReview}
							sx={{
								mt: 3,
								width: '100%',
							}}>
							Refresh
						</Button>
					</Container>
				)}

				{isReviewStage && !reviewIsError && reviewIsPending && (
					<Box
						mt={5}
						display='flex'
						justifyContent='center'>
						<CircularProgress size={50} />
					</Box>
				)}

				{isReviewStage && !reviewIsPending && !reviewIsError && (
					<Paper sx={{ p: 3 }}>
						<ReviewPanel
							items={mapOrderReviewResponseToDomain(reviewData!)}
							address={address}
							onBack={goBackToForm}
							onConfirm={handleSubmitOrder}
						/>
					</Paper>
				)}
			</Container>
		</>
	);
}

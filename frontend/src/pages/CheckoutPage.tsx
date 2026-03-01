import { Box, Container, Paper, Typography } from '@mui/material';
import CartSummary from '../components/checkout/cartSummary.tsx';
import AddressForm from '../components/checkout/addressForm.tsx';
import ReviewPanel from '../components/checkout/reviewPanel.tsx';
import useCheckoutLogic from '../hooks/pageLogic/useCheckoutLogic.tsx';
import type { AddressRequiredDto } from '../api/openApi/model';

export default function CheckoutPage() {
	const {
		cartItems,
		handleFormContinueToReview,
		form,
		goBackToForm,
		isFormStage,
		isReviewStage,
	} = useCheckoutLogic();

	const address: AddressRequiredDto = {
		houseNumber: form.watch('houseNumber'),
		city: form.watch('city'),
		street: form.watch('street'),
	};

	return (
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
			{isReviewStage && (
				<Paper sx={{ p: 3 }}>
					<ReviewPanel
						items={cartItems}
						address={address}
						onBack={goBackToForm}
						onConfirm={() => {}}
						loading={false}
					/>
				</Paper>
			)}
		</Container>
	);
}

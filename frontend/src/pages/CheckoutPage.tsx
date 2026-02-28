import { Box, Container, Paper, Typography } from '@mui/material';
import { useState } from 'react';
import { useCartStore } from '../stores/cartStore.ts';
import type { AddressRequiredDto } from '../api/openApi/model';
import CartSummary from '../components/checkout/cartSummary.tsx';
import AddressForm from '../components/checkout/addressForm.tsx';
import ReviewPanel from '../components/checkout/reviewPanel.tsx';

type Stage = 'form' | 'review';

export default function CheckoutPage() {
	const [stage, setStage] = useState<Stage>('form');
	const [address, setAddress] = useState<AddressRequiredDto | null>(null);
	const [loading, setLoading] = useState(false);

	const cartItems = useCartStore(state => state.items);

	const handleContinue = (values: AddressRequiredDto) => {
		setAddress(values);
		setStage('review');
	};

	const handleConfirm = async () => {
		try {
			setLoading(true);

			// TODO: pozovi backend ovde
			console.log('ORDER DATA:', {
				items: cartItems,
				address,
			});

			// simulacija
			await new Promise(res => setTimeout(res, 1000));

			alert('Order placed successfully!');
		} finally {
			setLoading(false);
		}
	};

	return (
		<Container
			maxWidth='lg'
			sx={{ py: 6 }}>
			<Typography
				variant='h4'
				mb={4}>
				Checkout
			</Typography>

			<Box
				display='flex'
				gap={4}
				alignItems='flex-start'
			>
				<Paper sx={{ flex: 1, p: 3 }}>
					{stage === 'form' ? (
						<AddressForm
							defaultValues={address ?? undefined}
							onContinue={handleContinue}
						/>
					) : (
						address && (
							<ReviewPanel
								items={cartItems}
								address={address}
								onBack={() => setStage('form')}
								onConfirm={handleConfirm}
								loading={loading}
							/>
						)
					)}
				</Paper>

				<Paper sx={{ flex: 1 }}>
					<CartSummary items={cartItems} />
				</Paper>
			</Box>
		</Container>
	);
}

import { Box, Button } from '@mui/material';
import { AddShoppingCart } from '@mui/icons-material';
import InputStepper from '../../shared/components/ui/inputStepper.tsx';

type Props = {
	inStock: boolean;
};

export default function ProductDetailsCta({ inStock }: Props) {
	return (
		<Box
			display='flex'
			justifyContent='center'
			gap={3}>
			<InputStepper
				title='Item Quantity'
				value={10}
				setValue={() => {}}
				disabled={!inStock}
			/>

			<Button
				startIcon={<AddShoppingCart />}
				variant='contained'
				size='large'
				disabled={!inStock}
				sx={{ flex: 1, fontWeight: 'bold', fontSize: 17 }}>
				Add to Cart
			</Button>
		</Box>
	);
}

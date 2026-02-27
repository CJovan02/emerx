import { Box, Button } from '@mui/material';
import { AddShoppingCart } from '@mui/icons-material';

type Props = {
	inStock: boolean;
};

export default function ProductDetailsCta({ inStock }: Props) {
	return (
		<Box
			display='flex'
			gap={2}>
			<Button
				startIcon={<AddShoppingCart />}
				variant='contained'
				size='large'
				disabled={!inStock}
				sx={{ flex: 1, fontWeight: 'bold' }}>
				Add to Cart
			</Button>
		</Box>
	);
}

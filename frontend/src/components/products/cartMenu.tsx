import useCartLogic from '../../hooks/pageLogic/useCartLogic.ts';
import useMenuState from '../../hooks/useMenuState.tsx';
import {
	Box,
	Button,
	Divider,
	IconButton,
	Menu,
	Stack,
	Typography,
} from '@mui/material';
import { Payment, ShoppingCart } from '@mui/icons-material';
import { getCartTotalPrice } from '../../domain/models/cartItem.ts';
import { useMemo } from 'react';
import { formatCurrency } from '../../utils/utils.ts';

export default function CartMenu() {
	const { openMenu, open, closeMenu, anchorEl } = useMenuState();
	const { cartItems } = useCartLogic();

	const totalPrice = useMemo(() => {
		return formatCurrency(getCartTotalPrice(cartItems));
	}, [cartItems]);

	return (
		<Box>
			<IconButton
				color='inherit'
				aria-controls={open ? 'cart-menu' : undefined}
				aria-haspopup='true'
				aria-expanded={open ? 'true' : undefined}
				onClick={openMenu}>
				<ShoppingCart sx={{ height: 30, width: 30 }} />
			</IconButton>
			<Menu
				id='cart-menu'
				anchorEl={anchorEl}
				open={open}
				onClose={closeMenu}
				anchorOrigin={{
					vertical: 'bottom',
					horizontal: 'right',
				}}
				transformOrigin={{
					vertical: 'top',
					horizontal: 'right',
				}}>
				<Box
					display='flex'
					flexDirection='column'
					height='100%'
					minWidth={320}>
					{/* Header */}
					<Box
						px={2}
						py={1.5}>
						<Typography fontWeight={600}>
							My Cart ({cartItems.length} items)
						</Typography>
					</Box>

					<Divider />

					{/* Scrollable Items Area */}
					<Box
						px={2}
						py={1}
						sx={{
							maxHeight: '50vh',
							overflowY: 'auto',
						}}>
						{/* OVDJE IDE LISTA CART ITEM-A */}
						<Typography
							variant='body2'
							color='text.secondary'>
							Cart items go here...
						</Typography>
					</Box>

					<Divider />

					{/* Footer */}
					<Box
						px={2}
						py={2}>
						<Stack spacing={2}>
							<Box
								display='flex'
								justifyContent='space-between'>
								<Typography fontWeight={500}>Subtotal</Typography>
								<Typography fontWeight={600}>{totalPrice}</Typography>
							</Box>

							<Button
								startIcon={<Payment />}
								variant='contained'
								fullWidth>
								Checkout
							</Button>
						</Stack>
					</Box>
				</Box>
			</Menu>
		</Box>
	);
}

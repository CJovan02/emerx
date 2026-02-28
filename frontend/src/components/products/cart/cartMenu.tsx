import useCartLogic from '../../../hooks/pageLogic/useCartLogic.ts';
import useMenuState from '../../../hooks/useMenuState.tsx';
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
import { getCartTotalPrice } from '../../../domain/models/cartItem.ts';
import { useMemo } from 'react';
import { formatCurrency } from '../../../utils/utils.ts';
import CartItemRow from './cartItemRow.tsx';
import { useNavigate } from 'react-router';
import { Routes } from '../../../shared/common/constants/routeNames.ts';

export default function CartMenu() {
	const { openMenu, open, closeMenu, anchorEl } = useMenuState();
	const { cartItems, removeFromCart } = useCartLogic();
	const navigate = useNavigate();

	const totalPrice = useMemo(() => {
		return formatCurrency(getCartTotalPrice(cartItems));
	}, [cartItems]);

	const navigateToProduct = (productId: string) =>
		navigate(Routes.ProductDetails(productId));

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
					minWidth={320}
					maxWidth={450}>
					{/* Header */}
					<Box
						px={2}
						py={1.5}>
						<Typography fontWeight={600}>
							My Cart{' '}
							<Typography
								component='span'
								fontWeight={300}
								color='textSecondary'>
								({cartItems.length} items)
							</Typography>
						</Typography>
					</Box>

					<Divider />

					{/* Scrollable Items Area */}
					{cartItems.length > 0 && (
						<>
							<Box
								// px={2}
								py={1}
								sx={{
									maxHeight: '50vh',
									overflowY: 'auto',
								}}>
								{cartItems.map(item => {
									return (
										<CartItemRow
											item={item}
											onRemove={removeFromCart}
											onClick={navigateToProduct}
										/>
									);
								})}
							</Box>
							<Divider />
						</>
					)}

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

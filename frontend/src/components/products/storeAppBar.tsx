import {
	AppBar,
	Box,
	Container,
	IconButton,
	InputAdornment,
	TextField,
	Toolbar,
	Typography,
} from '@mui/material';
import { FilterAlt, Search } from '@mui/icons-material';
import { Spacer } from '../../shared/components/ui/spacer.tsx';
import { Drawers } from '../../shared/common/constants/drawers.ts';
import useScreenSize from '../../hooks/useScreenSize.ts';
import { useStoreDrawerStore } from '../../stores/storeDrawerStore.tsx';
import AvatarMenu from './avatarMenu.tsx';
import { useLocation } from 'react-router';
import { Routes } from '../../shared/common/constants/routeNames.ts';
import CartMenu from './cart/cartMenu.tsx';
import { useEffect, useState } from 'react';
import { useProductFiltersStore } from '../../stores/productFiltersStore.ts';

const drawerWidth = Drawers.Store.Width;
const SEARCH_DEBOUNCE_MS = 350;

export default function StoreAppBar() {
	const { isDesktop } = useScreenSize();
	const openDrawer = useStoreDrawerStore(state => state.open);

	const location = useLocation();
	const isProductsRoute = location.pathname === Routes.Products;

	const setSearch = useProductFiltersStore(state => state.setSearch);
	const [inputValue, setInputValue] = useState('');

	useEffect(() => {
		const timer = setTimeout(() => {
			setSearch(inputValue);
		}, SEARCH_DEBOUNCE_MS);
		return () => clearTimeout(timer);
	}, [inputValue, setSearch]);

	useEffect(() => {
		if (!isProductsRoute) setInputValue('');
	}, [isProductsRoute]);

	return (
		<AppBar
			position='fixed'
			sx={
				isDesktop && isProductsRoute
					? { width: `calc(100% - ${drawerWidth}px)`, ml: `${drawerWidth}px` }
					: {}
			}>
			<Toolbar disableGutters>
				<Container maxWidth='lg'>
					<Box
						display='flex'
						alignItems='center'
						gap={1}>
						{!isDesktop && (
							<IconButton
								size='large'
								edge='start'
								color='inherit'
								aria-label='open filters drawer'
								onClick={openDrawer}
								sx={{ mr: 5 }}>
								<FilterAlt />
							</IconButton>
						)}

						{!isProductsRoute && (
							<Typography
								variant='h6'
								sx={{ fontWeight: 700, letterSpacing: '.2rem' }}>
								EMERX
							</Typography>
						)}

						{isProductsRoute && (
							<TextField
								value={inputValue}
								onChange={e => setInputValue(e.target.value)}
								placeholder='Search products…'
								size='small'
								sx={{
									flexGrow: 1,
									maxWidth: 480,
									'& .MuiOutlinedInput-root': {
										bgcolor: 'rgba(255,255,255,0.15)',
										borderRadius: 2,
										color: 'inherit',
										'& fieldset': { borderColor: 'rgba(255,255,255,0.3)' },
										'&:hover fieldset': { borderColor: 'rgba(255,255,255,0.6)' },
										'&.Mui-focused fieldset': { borderColor: 'white' },
									},
									'& input::placeholder': { color: 'rgba(255,255,255,0.7)' },
									input: { color: 'inherit' },
								}}
								slotProps={{
									input: {
										startAdornment: (
											<InputAdornment position='start'>
												<Search sx={{ color: 'rgba(255,255,255,0.7)' }} />
											</InputAdornment>
										),
									},
								}}
							/>
						)}

						<Spacer />

						<AvatarMenu />
						<CartMenu />
					</Box>
				</Container>
			</Toolbar>
		</AppBar>
	);
}

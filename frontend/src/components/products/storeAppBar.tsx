import { AppBar, Box, IconButton, Toolbar, Typography } from '@mui/material';
import { FilterAlt } from '@mui/icons-material';
import { Spacer } from '../reusable/spacer.tsx';
import StoreTabs from './storeTabs.tsx';
import { Drawers } from '../../shared/common/constants/drawers.ts';
import useScreenSize from '../../hooks/useScreenSize.ts';
import { useStoreDrawerStore } from '../../stores/storeDrawerStore.tsx';
import AvatarMenu from './avatarMenu.tsx';

const drawerWidth = Drawers.Store.Width;

export default function StoreAppBar() {
	const { isDesktop } = useScreenSize();
	const openDrawer = useStoreDrawerStore(state => state.open);

	return (
		<AppBar
			position='fixed'
			sx={
				isDesktop
					? { width: `calc(100% - ${drawerWidth}px)`, ml: `${drawerWidth}px` }
					: {}
			}>
			<Toolbar>
				{/* Drawer Toggle */}
				{!isDesktop && (
					<IconButton
						size='large'
						edge='start'
						color='inherit'
						aria-label='open filters drawer'
						onClick={openDrawer}
						sx={{
							mr: 5,
						}}>
						<FilterAlt />
					</IconButton>
				)}

				{/* Logo */}
				<Typography
					variant='h6'
					sx={{
						fontWeight: 700,
						letterSpacing: '.2rem',
					}}>
					EMERX
				</Typography>

				{/* Navigation Links */}
				<Box sx={{ display: 'flex', gap: 2, ml: 5 }}>
					<StoreTabs />
				</Box>

				<Spacer />

				<AvatarMenu />
			</Toolbar>
		</AppBar>
	);
}

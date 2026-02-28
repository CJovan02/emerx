import { Outlet, useLocation } from 'react-router';
import { Box, Toolbar} from '@mui/material';
import StoreAppBar from '../../components/products/storeAppBar';
import StoreDrawer from '../../components/products/storeDrawer.tsx';
import {Routes} from "../../shared/common/constants/routeNames.ts";

export default function StoreLayout() {
	const location = useLocation();
	const isProductsRoute = location.pathname === Routes.Products;

	return (
		<Box sx={{ display: 'flex' }}>
			<StoreAppBar />
			{isProductsRoute && <StoreDrawer />}

			<Box
				component='main'
				sx={{
					flexGrow: 1,
					p: 3,
				}}>
				<Toolbar />
				<Outlet />
			</Box>
		</Box>
	);
}

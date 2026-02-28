import { Tab, Tabs } from '@mui/material';
import { useLocation, useNavigate } from 'react-router';
import { Routes } from '../../shared/common/constants/routeNames.ts';

const StoreTabs = () => {
	const location = useLocation();
	const navigate = useNavigate();

	return (
		<Tabs
			value={location.pathname}
			aria-label='products tabs'
			onChange={(_, value) => navigate(value, { replace: true })}
			textColor='inherit'>
			<Tab
				value={Routes.Products}
				label='Products'
			/>
			<Tab
				value={Routes.Cart}
				label='Cart'
			/>
		</Tabs>
	);
};

export default StoreTabs;

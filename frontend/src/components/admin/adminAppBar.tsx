import { AppBar, Toolbar, Typography } from '@mui/material';
import { Drawers } from '../../shared/common/constants/drawers.ts';
import { Spacer } from '../../shared/components/ui/spacer.tsx';
import AvatarMenu from '../products/avatarMenu.tsx';
import { useEffect, useState } from 'react';
import { useLocation } from 'react-router';
import { Routes } from '../../shared/common/constants/routeNames.ts';

const drawerWidth = Drawers.Admin.Width;

export function AdminAppBar() {
	const [title, setTitle] = useState<string>('Admin Dashboard');
	const location = useLocation();

	useEffect(() => {
		if (location.pathname === Routes.Admin.Products) {
			setTitle('Manage Products');
		} else if (location.pathname === Routes.Admin.AdminsManagement) {
			setTitle('Manage Admin Roles');
		} else {
			setTitle('Admin Dashboard');
		}
	}, [location.pathname, setTitle]);

	return (
		<AppBar
			title='Admin Dashbord'
			position='fixed'
			sx={{ width: `calc(100% - ${drawerWidth}px)`, ml: `${drawerWidth}px` }}>
			<Toolbar>
				{/* Title */}
				<Typography
					variant='h6'
					sx={{
						fontWeight: 700,
					}}>
					{title}
				</Typography>

				<Spacer />

				<AvatarMenu />
			</Toolbar>
		</AppBar>
	);
}

import {
	Box,
	Divider,
	IconButton,
	ListItemIcon,
	ListItemText,
	Menu,
	MenuItem,
	Typography,
} from '@mui/material';
import {
	AccountCircle,
	AdminPanelSettings,
	Inventory,
	Logout,
	Person,
	Settings,
	ShoppingCart,
	Verified,
} from '@mui/icons-material';
import { auth } from '../../config/firebase.ts';
import { useEffect, useState } from 'react';
import * as React from 'react';
import { useUserStore } from '../../stores/userStore.ts';
import type AppUser from '../../domain/models/appUser.ts';
import { useLocation, useNavigate } from 'react-router';
import { Routes } from '../../shared/common/constants/routeNames.ts';

export default function AvatarMenu() {
	const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);
	const open = Boolean(anchorEl);
	const storeUser = useUserStore(state => state.user);
	// When we log out there will be a brief moment when where user store will have empty user and redirect will not trigger
	// leaving user menu to show undefined user info, we use this state as a cache for user info
	const [user, setUser] = useState<AppUser>();
	const navigate = useNavigate();
	const isAdminPage = useLocation().pathname.includes(Routes.Admin.Base);

	const handleOpenUserMenu = (event: React.MouseEvent<HTMLElement>) => {
		setAnchorEl(event.currentTarget);
	};

	const handleMenuAction = (callback?: () => void) => {
		setAnchorEl(null);

		if (callback) callback();
	};

	const navigateToAdmin = () => navigate(Routes.Admin.Base);
	const navigateToStore = () => navigate(Routes.Products);
	const navigateToMyProfile = () => navigate(Routes.MyProfile);

	useEffect(() => {
		if (!storeUser) return;

		setUser(storeUser);
	}, [storeUser]);

	return (
		<Box>
			<IconButton
				color='inherit'
				aria-controls={open ? 'account-menu' : undefined}
				aria-haspopup='true'
				aria-expanded={open ? 'true' : undefined}
				onClick={handleOpenUserMenu}>
				<Person sx={{ height: 32, width: 32 }} />
			</IconButton>

			<Menu
				id='account-menu'
				anchorEl={anchorEl}
				open={open}
				onClose={() => handleMenuAction()}
				anchorOrigin={{
					vertical: 'bottom',
					horizontal: 'right',
				}}
				transformOrigin={{
					vertical: 'top',
					horizontal: 'right',
				}}>
				<Box sx={{ px: 2, py: 1, mb: 1 }}>
					<Box
						display='flex'
						alignItems='center'
						mb={1}
						gap={1}>
						<AccountCircle fontSize='large' />
						<Typography
							variant='h6'
							fontWeight={600}
							display='flex'
							alignItems='center'>
							{`${user?.name} ${user?.surname[0]}.`}
							{user?.isAdmin && (
								<Verified
									color='primary'
									sx={{ ml: 0.5 }}
									fontSize='small'
								/>
							)}
						</Typography>
					</Box>
					<Typography
						variant='body2'
						color='text.secondary'>
						{user?.email}
					</Typography>
				</Box>
				<MenuItem onClick={() => handleMenuAction(navigateToMyProfile)}>
					<ListItemIcon>
						<Settings fontSize='small' />
					</ListItemIcon>
					<ListItemText primary='Edit Profile' />
				</MenuItem>
				<Divider />
				{user?.isAdmin && !isAdminPage && (
					<MenuItem onClick={() => handleMenuAction(navigateToAdmin)}>
						<ListItemIcon>
							<AdminPanelSettings fontSize='small' />
						</ListItemIcon>
						<ListItemText primary='Admin Dashboard' />
					</MenuItem>
				)}
				{user?.isAdmin && isAdminPage && (
					<MenuItem onClick={() => handleMenuAction(navigateToStore)}>
						<ListItemIcon>
							<Inventory fontSize='small' />
						</ListItemIcon>
						<ListItemText primary='Store Page' />
					</MenuItem>
				)}
				{user?.isAdmin && <Divider />}
				<MenuItem onClick={() => handleMenuAction()}>
					<ListItemIcon>
						<ShoppingCart fontSize='small' />
					</ListItemIcon>
					<ListItemText primary='Cart' />
				</MenuItem>
				<MenuItem
					onClick={() => handleMenuAction(() => auth.signOut())}
					sx={{ color: 'error.main' }}>
					<ListItemIcon>
						<Logout fontSize='small' />
					</ListItemIcon>

					<ListItemText primary='Logout' />
				</MenuItem>
			</Menu>
		</Box>
	);
}

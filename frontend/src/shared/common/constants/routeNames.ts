export const Routes = {
	Root: '/',
	Login: '/login',
	Register: '/register',
	MyProfile: '/my-profile',
	Products: '/products',
	Cart: '/cart',
	Admin: {
		Base: '/admin',
		Products: '/admin/products',
		AdminsManagement: '/admin/admins-management',
	},
} as const;

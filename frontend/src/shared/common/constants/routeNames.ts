export const Routes = {
	Root: '/',
	Login: '/login',
	Register: '/register',
	MyProfile: '/my-profile',
	Products: '/products',
	ProductDetails: (id: string) => `/products/${id}`,
	Cart: '/cart',
	Admin: {
		Base: '/admin',
		Products: '/admin/products',
		AdminsManagement: '/admin/admins-management',
	},
} as const;

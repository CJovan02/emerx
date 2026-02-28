import { CssBaseline, ThemeProvider } from '@mui/material';
import useAppTheme from '../config/useAppTheme.ts';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { createBrowserRouter, Navigate } from 'react-router';
import { RouterProvider } from 'react-router/dom';
import LoginPage from '../pages/LoginPage.tsx';
import RootLayout from '../pages/layouts/RootLayout.tsx';
import ProductsPage from '../pages/ProductsPage.tsx';
import { Routes } from '../shared/common/constants/routeNames.ts';
import RegisterPage from '../pages/RegisterPage.tsx';
import { SnackbarProvider } from 'notistack';
import StoreLayout from '../pages/layouts/StoreLayout.tsx';
import AdminLayout from '../pages/layouts/AdminLayout.tsx';
import AdminProductsPage from '../pages/admin/AdminProductsPage.tsx';
import AdminsManagementPage from '../pages/admin/AdminsManagementPage.tsx';
import PublicOnlyGuard from '../pages/guards/PublicOnlyGuard.tsx';
import RequireAuthGuard from '../pages/guards/RequireAuthGuard.tsx';
import RequireRolesGuard from '../pages/guards/RequireRolesGuard.tsx';
import { UserRoles } from '../domain/models/userRoles.ts';
import MyProfilePage from '../pages/MyProfilePage.tsx';
import { ProductDetailsPage } from '../pages/ProductDetailsPage.tsx';
import CheckoutPage from '../pages/CheckoutPage.tsx';

const router = createBrowserRouter([
	{
		path: Routes.Root,
		Component: RootLayout,
		children: [
			// Public pages
			{
				Component: PublicOnlyGuard,
				children: [
					{
						index: true,
						Component: () => (
							<Navigate
								to={Routes.Login}
								replace
							/>
						),
					},
					{
						path: Routes.Login,
						Component: LoginPage,
					},
					{
						path: Routes.Register,
						Component: RegisterPage,
					},
				],
			},

			// Private routes
			{
				Component: RequireAuthGuard,
				children: [
					// Store layout
					{
						Component: StoreLayout,
						children: [
							{
								path: Routes.Products,
								children: [
									{
										index: true,
										Component: ProductsPage,
									},
									{
										path: ':id',
										Component: ProductDetailsPage,
									},
								],
							},
							{
								path: Routes.Cart,
								Component: ProductsPage,
							},
							{
								path: Routes.MyProfile,
								Component: MyProfilePage,
							},
							{
								path: Routes.Checkout,
								Component: CheckoutPage,
							},
						],
					},
				],
			},

			// Admin layout
			{
				element: <RequireRolesGuard roles={[UserRoles.Admin]} />,
				children: [
					{
						path: Routes.Admin.Base,
						Component: AdminLayout,
						children: [
							{
								index: true,
								Component: () => (
									<Navigate
										to={Routes.Admin.Products}
										replace
									/>
								),
							},
							{
								path: Routes.Admin.Products,
								Component: AdminProductsPage,
							},
							{
								path: Routes.Admin.AdminsManagement,
								Component: AdminsManagementPage,
							},
						],
					},
				],
			},
		],
	},
]);

const queryClient = new QueryClient();

const Providers = () => {
	const theme = useAppTheme();

	return (
		<ThemeProvider theme={theme}>
			<CssBaseline />
			<QueryClientProvider client={queryClient}>
				<SnackbarProvider>
					<RouterProvider router={router} />
				</SnackbarProvider>
			</QueryClientProvider>
		</ThemeProvider>
	);
};

export default Providers;

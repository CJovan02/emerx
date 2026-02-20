import {CssBaseline, ThemeProvider} from '@mui/material';
import useTheme from '../config/theme.ts';
import {QueryClient, QueryClientProvider} from '@tanstack/react-query';
import {createBrowserRouter} from 'react-router';
import {RouterProvider} from 'react-router/dom';
import LoginPage from '../pages/LoginPage.tsx';
import RootLayout from '../pages/layouts/RootLayout.tsx';
import SplashPage from '../pages/SplashPage.tsx';
import ProductsPage from '../pages/ProductsPage.tsx';
import {Routes} from '../shared/common/constants/routeNames.ts';
import RegisterPage from '../pages/RegisterPage.tsx';
import {SnackbarProvider} from 'notistack';
import StoreLayout from "../pages/layouts/StoreLayout.tsx";

const router = createBrowserRouter([
    {
        path: Routes.Root,
        Component: RootLayout,
        children: [
            {
                index: true,
                Component: SplashPage,
            },
            {
                path: Routes.Login,
                Component: LoginPage,
            },
            {
                path: Routes.Register,
                Component: RegisterPage,
            },
            {
                Component: StoreLayout,
                children: [
                    {
                        path: Routes.Products,
                        Component: ProductsPage,
                    },
                    {
                        path: Routes.Cart,
                        Component: ProductsPage
                    }
                ]
            }
        ],
    },
]);

const queryClient = new QueryClient();

const Providers = () => {
    const theme = useTheme();

    return (
        <ThemeProvider theme={theme}>
            <CssBaseline/>
            <QueryClientProvider client={queryClient}>
                <SnackbarProvider>
                    <RouterProvider router={router}/>
                </SnackbarProvider>
            </QueryClientProvider>
        </ThemeProvider>
    );
};

export default Providers;

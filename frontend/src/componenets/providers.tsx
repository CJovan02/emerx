import {CssBaseline, ThemeProvider} from '@mui/material';
import useTheme from "../config/theme.ts";
import {
    QueryClient, QueryClientProvider,
} from '@tanstack/react-query'
import {createBrowserRouter} from "react-router";
import {RouterProvider} from "react-router/dom";
import LoginPage from "../pages/LoginPage.tsx";
import RootLayout from "../pages/layouts/RootLayout.tsx";
import SplashPage from "../pages/SplashPage.tsx";
import StorePage from "../pages/StorePage.tsx";

const router = createBrowserRouter([
    {
        path: "/",
        Component: RootLayout,
        children: [
            {
                index: true,
                Component: SplashPage,
            },
            {
                path: "/login",
                Component: LoginPage,
            },
            {
                path: '/store',
                Component: StorePage,
            }
        ]
    },
]);

const queryClient = new QueryClient()

const Providers = () => {
    const theme = useTheme();

    return (
        <ThemeProvider theme={theme}>
            <CssBaseline/>
            <QueryClientProvider client={queryClient}>
                <RouterProvider router={router}/>
            </QueryClientProvider>
        </ThemeProvider>
    );
};

export default Providers;

import {CssBaseline, ThemeProvider} from '@mui/material';
import type {ReactNode} from 'react';
import useTheme from "../config/theme.ts";
import {
    QueryClient, QueryClientProvider,
} from '@tanstack/react-query'

const queryClient = new QueryClient()

const Providers = ({children}: { children: ReactNode }) => {
    const theme = useTheme();

    return (
        <ThemeProvider theme={theme}>
            <CssBaseline/>
            <QueryClientProvider client={queryClient}>
                {children}
            </QueryClientProvider>
        </ThemeProvider>
    );
};

export default Providers;

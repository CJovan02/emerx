import {CssBaseline, ThemeProvider} from '@mui/material';
import type {ReactNode} from 'react';
import useTheme from "../config/theme.ts";

const Providers = ({children}: { children: ReactNode }) => {
    const theme = useTheme();

    return (
        <ThemeProvider theme={theme}>
            <CssBaseline/>
            {children}
        </ThemeProvider>
    );
};

export default Providers;

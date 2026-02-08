declare module '@mui/material/styles' {
    interface Theme {
        custom: {
            gradients: {
                background: string;
            };
        };
    }
    interface ThemeOptions {
        custom?: {
            gradients?: {
                background?: string;
            };
        };
    }
}

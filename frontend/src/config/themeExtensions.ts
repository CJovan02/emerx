declare module '@mui/material/styles' {
    interface Palette {
        surface: {
            base: string;
            subtle: string;
            muted: string;
            elevated: string;
            hover: string;
        };
        border: {
            subtle: string;
            strong: string;
        };
        brand: {
            soft: string;
            muted: string;
        };
    }

    interface PaletteOptions {
        surface?: {
            base?: string;
            subtle?: string;
            muted?: string;
            elevated?: string;
            hover?: string;
        };
        border?: {
            subtle?: string;
            strong?: string;
        };
        brand?: {
            soft?: string;
            muted?: string;
        };
    }

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

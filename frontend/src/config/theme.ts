import {createTheme, lighten} from '@mui/material';
import {Colors} from "../shared/common/constants/colors.ts";

function useTheme() {
    const theme = createTheme({
        palette: {
            primary: {
                main: Colors.Primary.Main,
            },
            secondary: {
                main: Colors.Secondary.Main,
            },
            background: {
                default: lighten(Colors.Primary.Main, 0.97),
                paper: lighten(Colors.Primary.Main, 0.95),
            }
        },

        shape: {
            borderRadius: 12,
        },
        typography: {
            fontFamily: `'Inter', 'Roboto', 'Helvetica', 'Arial', sans-serif`,
            button: {
                textTransform: 'none',
                fontWeight: 600,
            },
        },
        components: {

            MuiButton: {
                defaultProps: {
                    variant: 'contained',
                },
            },
            MuiCard: {
                styleOverrides: {
                    root: {
                        borderRadius: 16,
                    },
                },
            },
            MuiCardHeader: {
                styleOverrides: {
                    title: {
                        fontWeight: 600,
                        marginBottom: 8
                    }
                }
            },
            MuiTextField: {
                defaultProps: {
                    variant: 'outlined',
                },
            },
        },
    });

    return theme;
}

export default useTheme;
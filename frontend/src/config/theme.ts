import {createTheme} from '@mui/material';
import {blue, indigo, purple} from "@mui/material/colors";

function useTheme() {
    const theme = createTheme({
        palette: {
            primary: {
                main: blue[700],
            },
            secondary: {
                main: purple[500],
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
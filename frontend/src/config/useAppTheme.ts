import {alpha, createTheme, lighten} from '@mui/material';
import {Colors} from "../shared/common/constants/colors.ts";

function useAppTheme() {
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
            },

            surface: {
                base: "#ffffff",
                subtle: alpha(Colors.Primary.Main, 0.03),
                muted: alpha(Colors.Primary.Main, 0.06),
                elevated: alpha("#000", 0.02),
                hover: alpha(Colors.Primary.Main, 0.08),
            },

            brand: {
                soft: alpha(Colors.Primary.Main, 0.12),
                muted: alpha(Colors.Primary.Main, 0.06),
            },

            // border: {
            //     subtle: "#e2e8f0",
            //     strong: "#cbd5e1",
            // },

            // text: {
            //     primary: "#0f172a",
            //     secondary: "#475569",
            // },
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
        custom: {
            gradients: {
                background: `linear-gradient(
                    135deg,
                    ${alpha(Colors.Primary.Main, 0.08)},
                    ${alpha(Colors.Secondary.Main, 0.08)}
                  )`
            }
        },
        components: {
            MuiDivider: {
                styleOverrides: {
                    root: ({ theme }) => ({
                        borderColor: theme.palette.text.disabled,
                    }),
                },
            },
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
            MuiListItemIcon: {
                styleOverrides: {
                    root: {
                        color: "inherit",
                    },
                },
            },
            // MuiAppBar: {
            //     styleOverrides: {
            //         root: ({theme}) => ({
            //             backgroundColor: theme.palette.surface.muted,
            //             color: theme.palette.text.primary,
            //         })
            //     }
            // }
        },
    });

    return theme;
}

export default useAppTheme;
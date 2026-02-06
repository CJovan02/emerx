import {createTheme} from '@mui/material';
import {blue, purple} from "@mui/material/colors";

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
        components: {
            MuiButton: {
                defaultProps: {
                    variant: 'contained',
                }
            }
        }
    });

    return theme;
}

export default useTheme;
import { createTheme } from '@mui/material';
import { Colors } from '../shared/common/constants/colors';

export const theme = createTheme({
	palette: {
		primary: {
			main: Colors.Primary.Main,
			light: Colors.Primary.Light,
			dark: Colors.Primary.Dark,
		},
	},
});

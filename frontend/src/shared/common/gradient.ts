import {alpha} from '@mui/material/styles';
import {type Theme} from '@mui/material';

export const bgGradient = (theme: Theme) => {
    return `linear-gradient(
    135deg,
    ${alpha(theme.palette.primary.main, 0.08)},
    ${alpha(theme.palette.secondary.main, 0.08)}
  )`;
};

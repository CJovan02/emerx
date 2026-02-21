import {useMediaQuery, useTheme} from "@mui/material";

export default function useScreenSize() {
    const theme = useTheme();
    const isDesktop = useMediaQuery(theme.breakpoints.up("md"));

    return { isDesktop };
}
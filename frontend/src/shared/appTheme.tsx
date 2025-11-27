import { ThemeProvider } from "@mui/material";
import { theme } from "../config/theme";
import type { ReactNode } from "react";

const AppTheme = ({ children }: { children: ReactNode }) => {
  return <ThemeProvider theme={theme}>{children}</ThemeProvider>;
};

export default AppTheme;

import CircularProgress from "@mui/material/CircularProgress";
import {Box} from "@mui/material";

export default function SplashPage() {
    return (
        <Box minHeight={"100vh"} display={"flex"} alignItems="center" justifyContent={"center"}>
            <CircularProgress size={60}/>
        </Box>
    )
}
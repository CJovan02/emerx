import {Box, Button} from "@mui/material";
import {auth} from "../config/firebase.ts";

export default function StorePage() {
    return (
        <Box display='flex' alignItems='center' justifyContent='center' minHeight='100vh'>
            <Button onClick={() => auth.signOut()}>
                Sign Out
            </Button>
        </Box>
    )
}
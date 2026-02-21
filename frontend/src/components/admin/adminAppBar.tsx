import {AppBar, Toolbar, Typography} from "@mui/material";
import {Drawers} from "../../shared/common/constants/drawers.ts";
import {Spacer} from "../reusable/spacer.tsx";
import AvatarMenu from "../products/avatarMenu.tsx";

const drawerWidth = Drawers.Admin.Width;

export function AdminAppBar() {
    return (
        <AppBar
            title="Admin Dashbord"
            position='fixed'
            sx={{width: `calc(100% - ${drawerWidth}px)`, ml: `${drawerWidth}px`}}
        >
            <Toolbar>
                {/* Title */}
                <Typography
                    variant="h6"
                    sx={{
                        fontWeight: 700,
                        //letterSpacing: ".2rem",
                    }}
                >
                    Admin Dashboard
                </Typography>

                <Spacer />

                <AvatarMenu />
            </Toolbar>
        </AppBar>
    )
}
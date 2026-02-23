import AdminDrawer from "../../components/admin/adminDrawer.tsx";
import {AdminAppBar} from "../../components/admin/adminAppBar.tsx";
import {Box, Toolbar} from "@mui/material";
import {Outlet} from "react-router/internal/react-server-client";

export default function AdminLayout() {
    return (
        <Box display='flex'>
            <AdminDrawer/>
            <AdminAppBar/>

            <Box
                component='main'
                sx={{
                    flexGrow: 1,
                    p: 3,
                }}
            >
                <Toolbar/>
                <Outlet/>
            </Box>
        </Box>
    )
}
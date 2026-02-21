import AdminDrawer from "../../components/admin/adminDrawer.tsx";
import {AdminAppBar} from "../../components/admin/adminAppBar.tsx";
import {Box} from "@mui/material";
import {Outlet} from "react-router/internal/react-server-client";
import {useUserStore} from "../../stores/userStore.ts";
import {Navigate} from "react-router";
import {Routes} from "../../shared/common/constants/routeNames.ts";

export default function AdminLayout() {
    const {user} = useUserStore();

    if (!user || !user.isAdmin) {
        return <Navigate to={Routes.Products} replace/>;
    }

    return (
        <>
            <AdminDrawer/>
            <AdminAppBar/>

            <Box
                component='main'
                sx={{
                    flexGrow: 1,
                    p: 3
                }}
            >
                <Outlet/>
            </Box>
        </>
    )
}
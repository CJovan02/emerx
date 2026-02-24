import {Divider, Drawer, List, Toolbar, Typography} from "@mui/material";
import {Drawers} from "../../shared/common/constants/drawers.ts";
import {AdminPanelSettings, Inventory, SupervisorAccount} from "@mui/icons-material";
import DrawerNavLinkButton from "../../shared/components/ui/drawerNavLinkButton.tsx";
import {Routes} from "../../shared/common/constants/routeNames.ts";

const drawerWidth = Drawers.Admin.Width;

export default function AdminNavigationDrawer() {
    return (
        <Drawer
            variant="permanent"
            sx={{
                width: drawerWidth,
                flexShrink: 0,
                '& .MuiDrawer-paper': {
                    width: drawerWidth,
                    boxSizing: 'border-box',
                },
            }}
        >
            <Toolbar disableGutters
                sx={{
                    px: 2,
                    gap: 3,
                    alignItems: "center",
                }}
            >
                <AdminPanelSettings fontSize='large' color='primary' />
                <Typography color='primary' variant='h6' fontWeight={700}>
                    Dashboard
                </Typography>
            </Toolbar>
            <Divider/>

            <List>
                <DrawerNavLinkButton
                    to={Routes.Admin.Products}
                    icon={<Inventory/>}
                    text='Products'
                />
                <DrawerNavLinkButton
                    to={Routes.Admin.AdminsManagement}
                    icon={<SupervisorAccount/>}
                    text='Admins Management'
                />

            </List>
        </Drawer>

    )
}
import {Divider, Drawer, List, ListItemButton, ListItemIcon, ListItemText, Toolbar} from "@mui/material";
import {Drawers} from "../../shared/common/constants/drawers.ts";
import {AdminPanelSettings, Inventory, SupervisorAccount} from "@mui/icons-material";

const drawerWidth = Drawers.Admin.Width;

export default function AdminDrawer() {
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
            <Toolbar />
            <Divider />

            <List>
                <ListItemButton>
                    <ListItemIcon>
                        <AdminPanelSettings />
                    </ListItemIcon>
                    <ListItemText primary="Dashboard" />
                </ListItemButton>

                <ListItemButton>
                    <ListItemIcon>
                        <Inventory />
                    </ListItemIcon>
                    <ListItemText primary="Products" />
                </ListItemButton>

                <ListItemButton>
                    <ListItemIcon>
                        <SupervisorAccount />
                    </ListItemIcon>
                    <ListItemText primary="Control Admin Roles" />
                </ListItemButton>
            </List>
        </Drawer>

    )
}
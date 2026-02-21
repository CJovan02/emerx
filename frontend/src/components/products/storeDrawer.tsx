import {Divider, Drawer, List, Toolbar} from "@mui/material";
import {useStoreDrawerStore} from "../../stores/storeDrawerStore.tsx";
import {Drawers} from "../../shared/common/constants/drawers.ts";
import useScreenSize from "../../hooks/useScreenSize.tsx";

const drawerWidth = Drawers.Store.Width;

export default function StoreDrawer() {
    const { isDesktop } = useScreenSize();
    const close = useStoreDrawerStore(state => state.close);
    const isOpen = useStoreDrawerStore(state => state.isOpen);

    return (
        <Drawer
            open={isDesktop ? true : isOpen}
            onClose={close}
            variant={isDesktop ? "permanent" : "temporary"}
            sx={{
                width: drawerWidth,
                flexShrink: 0,
                '& .MuiDrawer-paper': {
                    width: drawerWidth,
                    boxSizing: 'border-box',
                },
            }}
            anchor="left"
        >
            <Toolbar/>
            <Divider/>
            <List>
                {/*<ListItemButton>jole</ListItemButton>*/}
            </List>
        </Drawer>

    )
}
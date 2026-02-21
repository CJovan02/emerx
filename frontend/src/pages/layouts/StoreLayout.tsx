import {Outlet} from "react-router";
import {Box} from "@mui/material";
import StoreAppBar from "../../components/products/storeAppBar";
import StoreDrawer from "../../components/products/storeDrawer.tsx";

export default function StoreLayout() {
    return (
        <Box sx={{display: "flex"}}>

            <StoreAppBar/>
            <StoreDrawer/>

            <Box
                component="main"
                sx={{
                    flexGrow: 1,
                    p: 3,
                }}
            >
                <Outlet/>

            </Box>
        </Box>
    );
}
import {Outlet} from "react-router/internal/react-server-client";
import StoreAppBar from "../../components/products/storeAppBar.tsx";

export default function StoreLayout() {
    return (
        <>
            <StoreAppBar/>
            <Outlet/>
        </>
    );
};
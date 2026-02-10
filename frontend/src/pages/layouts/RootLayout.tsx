import AuthUserSync from "../../componenets/authUserSync.tsx";
import AuthFlowRedirection from "../../componenets/authFlowRedirection.tsx";
import {Outlet} from "react-router/internal/react-server-client";

function RootLayout() {
    return (
        <>
            <AuthUserSync/>
            <AuthFlowRedirection/>
            <Outlet/>
        </>
    )
}

export default RootLayout;
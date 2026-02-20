import {Outlet} from "react-router/internal/react-server-client";
import AuthUserSync from "../../components/authFlow/authUserSync.tsx";
import AuthFlowRedirection from "../../components/authFlow/authFlowRedirection.tsx";

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
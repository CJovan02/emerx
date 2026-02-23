import {useUserStore} from "../../stores/userStore.ts";
import {Navigate, Outlet, useLocation} from "react-router";
import {Routes} from "../../shared/common/constants/routeNames.ts";

// If user is logged in inside this layout, he will get redirected
export default function PublicOnlyGuard() {
    const user = useUserStore(store => store.user);
    const isUserLoading = useUserStore(store => store.isLoading);

    const location = useLocation();
    const from = location.state?.from?.pathname ?? Routes.Products;

    if (isUserLoading) return null;

    if (user)
        return <Navigate to={from} replace/>

    return <Outlet/>
}
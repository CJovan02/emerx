import {useEffect} from "react";
import {useNavigate} from "react-router";
import {useUserStore} from "../stores/userStore.ts";

// This component is used to redirect user depending on the user store state
// user is logged in -> redirect to store page
// user is not logged in -> redirect to login page
// NOTE: the reason I used user store instead of auth state is that we need to make user is logged in as well as
// that the user data is fetched from the server. User store is also in sync with auth state so it's fine
function AuthFlowRedirection() {
    const navigate = useNavigate();
    const user = useUserStore(state => state.user)

    useEffect(() => {
        if (!user) {
            navigate("/login", {replace: true});
            return;
        }
        navigate("/store", {replace: true});
    }, [user])

    return null;
}

export default AuthFlowRedirection;
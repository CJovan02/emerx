import {useEffect, useState} from "react";
import {auth} from "../config/firebase.ts";
import {useUserStore} from "../stores/userStore.ts";
import {userGetSelf} from "../api/openApi/user/user.ts";
import {Backdrop} from "@mui/material";
import CircularProgress from "@mui/material/CircularProgress";

// This component is used to sync firebase user state changes (sign in/sign out) with the user store
// User signs in -> call the server and fill in user store
// User signs out -> clear user store
function AuthUserSync() {
    const setUser = useUserStore((state) => state.setUser);
    const clearUser = useUserStore((state) => state.clearUser);
    const [loading, setLoading] = useState(false);

    useEffect(() => {
        const unsubscribe = auth.onAuthStateChanged(async (user) => {
            if (!user) {
                console.log("User signed out")
                clearUser();
                return;
            }
            console.log("User signed in")

            try {
                setLoading(true);

                // We directly call the endpoint, react query for this situation complicates stuff
                const data = await userGetSelf();
                setUser(data);

            } catch (error) {
                // TODO show snackbar
                // This will reset user store, it's written 10 above :)
                await auth.signOut();
                console.error(error);
            } finally {
                setLoading(false);
            }
        })

        return () => unsubscribe()
    }, []);

    return (
        <Backdrop open={loading}>
            <CircularProgress size={50} />
        </Backdrop>
    );
}

export default AuthUserSync;
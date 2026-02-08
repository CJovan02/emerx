import {useEffect} from "react";
import {auth} from "../config/firebase.ts";
import {useUserStore} from "../stores/userStore.ts";
import {userGetSelf} from "../api/openApi/user/user.ts";

// This component is used to sync firebase user state changes (sign in/sign out) with the user store
// User signs in -> call the server and fill in user store
// User signs out -> clear user store
function AuthUserSync() {
    const setUser = useUserStore((state) => state.setUser);
    const clearUser = useUserStore((state) => state.clearUser);

    useEffect(() => {
        const unsubscribe = auth.onAuthStateChanged(async (user) => {
            if (!user) {
                console.log("User signed out")
                clearUser();
                return;
            }
            console.log("User signed in")

            try {
                // We directly call the endpoint, react query for this situation complicates stuff
                const data = await userGetSelf();
                setUser(data);
            } catch (error) {
                // TODO show snackbar
                // This will reset user store, it's written 10 above :)
                await auth.signOut();
                console.error(error);
            }
        })

        return () => unsubscribe()
    }, []);

    return null;
}

export default AuthUserSync;
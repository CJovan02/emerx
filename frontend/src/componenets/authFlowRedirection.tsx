import {useEffect} from "react";
import {auth} from "../config/firebase.ts";
import {useNavigate} from "react-router";

// This component is used to redirect user depending on the auth state
// user is logged in -> redirect to store page
// user is not logged in -> redirect to login page
function AuthFlowRedirection() {
    const navigate = useNavigate();

    useEffect(() => {
        const unsubscribe = auth.onAuthStateChanged((user) => {
            if (!user) {
                navigate("/login", {replace: true});
                return;
            }

            navigate("/store", {replace: true});
        })

        return () => unsubscribe();
    }, [])

    return null;
}

export default AuthFlowRedirection;
import { useEffect } from 'react';
import {useLocation, useNavigate} from 'react-router';
import { useUserStore } from '../../stores/userStore.ts';
import { Routes } from '../../shared/common/constants/routeNames.ts';

// This component is used to redirect user depending on the user store state
// user is logged in -> redirect to store page
// user is not logged in -> redirect to login page
// NOTE: the reason I used user store instead of auth state is that we need to make sure that user is logged in as well as
// that the user data is fetched from the server. AppUser store is also in sync with auth state so it's fine
function AuthFlowRedirection() {
	const navigate = useNavigate();
	const location = useLocation();
	const user = useUserStore(state => state.user);
	const isLoading = useUserStore(state => state.isLoading);

	useEffect(() => {
		if(isLoading) return;

		if (!user) {
			navigate(Routes.Login, { replace: true });
			return;
		}

		// If user opened application directly through specific url (ex. /dashboard/products)
		// we don't want to interrupt him and redirect him to Products page.
		// Redirection to Products when user logs in is done only when user is in /login page
		if (location.pathname !== Routes.Login && location.pathname !== '/') return;

		navigate(Routes.Products, { replace: true });
	}, [user, isLoading]);

	return null;
}

export default AuthFlowRedirection;

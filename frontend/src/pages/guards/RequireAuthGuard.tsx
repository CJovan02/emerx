import { useUserStore } from '../../stores/userStore.ts';
import { Navigate, useLocation } from 'react-router';
import { Routes } from '../../shared/common/constants/routeNames.ts';
import { Outlet } from 'react-router/internal/react-server-client';

// If user is not logged in inside this layout, he will be redirected to /login
export default function RequireAuthGuard() {
	const user = useUserStore(store => store.user);
	const isUserLoading = useUserStore(store => store.isLoading);
	const location = useLocation();

	if (isUserLoading) return null;

	if (!user)
		return (
			<Navigate
				to={Routes.Login}
				state={{ from: location }}
				replace
			/>
		);

	return <Outlet />;
}

import { useUserStore } from '../../stores/userStore.ts';
import { Navigate, useLocation } from 'react-router';
import { Routes } from '../../shared/common/constants/routeNames.ts';
import { Outlet } from 'react-router/internal/react-server-client';

type Props = {
	roles: number[];
};

// Redirects user to login if he is not authorized and,
// It required certain user roles in order to access this layout
export default function RequireRolesGuard({ roles }: Props) {
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

	const hasRequiredRole = roles.some(role => user.roles.includes(role));

	if (!hasRequiredRole)
		return (
			<Navigate
				to={Routes.Products}
				replace
			/>
		);

	return <Outlet />;
}

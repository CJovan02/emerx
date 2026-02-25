import { Outlet } from 'react-router/internal/react-server-client';
import AuthUserSync from '../../components/authFlow/authUserSync.tsx';

function RootLayout() {
	return (
		<>
			<AuthUserSync />
			<Outlet />
		</>
	);
}

export default RootLayout;

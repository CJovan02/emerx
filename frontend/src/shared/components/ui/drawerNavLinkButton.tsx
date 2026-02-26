import { ListItemButton, ListItemIcon, ListItemText } from '@mui/material';
import { type ReactNode, useEffect, useState } from 'react';
import { Link, useLocation } from 'react-router';

type Props = {
	to: string;
	icon: ReactNode;
	text: string;
};

export default function DrawerNavLinkButton({
	to,
	icon,
	text,
}: Props): ReactNode {
	const location = useLocation();
	const [isActive, setIsActive] = useState(false);

	useEffect(() => {
		const value = location.pathname.startsWith(to);
		setIsActive(value);
	}, [location.pathname]);

	return (
		<ListItemButton
			component={Link}
			to={to}
			selected={isActive}
			replace>
			<ListItemIcon>{icon}</ListItemIcon>
			<ListItemText primary={text} />
		</ListItemButton>
	);
}

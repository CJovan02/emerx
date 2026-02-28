import { useCallback, useState } from 'react';
import * as React from 'react';

export default function useMenuState() {
	const [anchorEl, setAnchorEl] = useState<HTMLElement | null>(null);
	const open = Boolean(anchorEl);

	const openMenu = useCallback(
		(event: React.MouseEvent<HTMLElement>) => {
			setAnchorEl(event.currentTarget);
		},
		[setAnchorEl]
	);

	const closeMenu = useCallback(() => {
		setAnchorEl(null);
	}, [setAnchorEl]);

	return {
		anchorEl,
		open,
		openMenu,
		closeMenu,
	};
}

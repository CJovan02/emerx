import { Box, Typography } from '@mui/material';
import * as React from 'react';

export default function AddressFieldInfo({
	icon,
	value,
}: {
	value: string;
	icon: React.ReactNode;
}) {
	return (
		<Box
			display='flex'
			alignItems='center'
			gap={1}>
			{icon}
			<Typography variant='body1'>{value}</Typography>
		</Box>
	);
}

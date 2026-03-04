import { Box, Typography } from '@mui/material';

export default function ProductStock({
	stock,
	size = 'normal',
}: {
	stock: number;
	size?: 'normal' | 'small';
}) {
	const color = stock > 0 ? 'success.main' : 'error.main';
	const isSmall = size === 'small';

	const text =
		stock > 0
			? stock < 10
				? `Only ${stock} left in stock`
				: 'In stock'
			: 'Out of stock';

	return (
		<Box
			display='flex'
			alignItems='center'
			gap={1}>
			{/* Dot */}
			<Box
				sx={{
					width: isSmall ? 7 : 8,
					height: isSmall ? 7 : 8,
					borderRadius: '50%',
					bgcolor: color,
				}}
			/>
			{/* Text */}
			<Typography
				variant={isSmall ? 'caption' : 'body2'}
				fontWeight={600}
				color={color}>
				{text}
			</Typography>
		</Box>
	);
}

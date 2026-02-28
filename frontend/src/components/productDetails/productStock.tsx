import { Box, Typography } from '@mui/material';

export default function ProductStock({ stock }: { stock: number }) {
	const color = stock > 0 ? 'success.main' : 'error.main';

	const text =
		stock > 0
			? stock < 10
				? `Only ${stock} left in stock`
				: 'In stock'
			: 'Out of stock';

	return (
		<Box display="flex" alignItems="center" gap={1} mb={3}>
			{/* Dot */}
			<Box
				sx={{
					width: 8,
					height: 8,
					borderRadius: '50%',
					bgcolor: color,
				}}
			/>
			{/* Text */}
			<Typography variant="body2" color={color}>
				{text}
			</Typography>
		</Box>
	);
}
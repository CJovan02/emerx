import { Box, IconButton, Typography } from '@mui/material';
import { Add, Remove } from '@mui/icons-material';

export default function InputStepper({
	value,
	setValue,
	title,
	disabled,
}: {
	value: number;
	setValue: (q: number) => void;
	disabled?: boolean;
	title?: string;
}) {
	const handleDecrement = () => setValue(Math.max(1, value - 1));
	const handleIncrement = () => setValue(value + 1);

	return (
		<Box
			display='flex'
			alignItems='center'
			justifyItems='center'
			flexDirection='column'
			gap={1}>
			{title && <Typography variant='body2'>{title}</Typography>}
			<Box
				display='flex'
				alignItems='center'
				gap={1.5}
				bgcolor='surface.muted'
				p={0.6}
				borderRadius={3}>
				<IconButton
					color='primary'
					size='small'
					onClick={handleDecrement}
					disabled={!disabled ? value <= 1 : true}>
					<Remove />
				</IconButton>

				<Typography
					minWidth={24}
					color={disabled ? 'textDisabled' : 'textPrimary'}
					textAlign='center'>
					{value}
				</Typography>

				<IconButton
					color='primary'
					size='small'
					disabled={disabled}
					onClick={handleIncrement}>
					<Add />
				</IconButton>
			</Box>
		</Box>
	);
}

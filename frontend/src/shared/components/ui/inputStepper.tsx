import { Box, IconButton, Typography } from '@mui/material';
import { Add, Remove } from '@mui/icons-material';

export default function InputStepper({
	value,
	onIncrement,
	onDecrement,
	title,
	disabled,
	isMaximum
}: {
	value: number;
	isMaximum?: boolean;
	disabled?: boolean;
	onIncrement: () => void;
	onDecrement: () => void;
	title?: string;
}) {
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
					onClick={onDecrement}
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
					disabled={disabled || isMaximum}
					onClick={onIncrement}>
					<Add />
				</IconButton>
			</Box>
		</Box>
	);
}

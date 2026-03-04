import { Box, Stack, Tooltip, Typography } from '@mui/material';
import { FormProvider, type UseFormReturn } from 'react-hook-form';
import TextInput from '../../shared/components/ui/textInput.tsx';
import { Info } from '@mui/icons-material';
import * as React from 'react';

type Props = {
	form: UseFormReturn<{
		fullName: string;
		email: string;
		city: string;
		street: string;
		houseNumber: string;
	}>;
	onSubmit: (e?: React.BaseSyntheticEvent) => Promise<void>;
};

export default function AddressForm({ form, onSubmit }: Props) {
	return (
		<Box>
			<Box
				display='flex'
				alignItems='center'
				gap={1}
				mb={4}>
				<Typography
					variant='h5'
					fontWeight={600}>
					Shipping Information
				</Typography>
				<Tooltip
					placement='top'
					title='Some of the shipping information is picked up from your profile. You can change address if you need to.'>
					<Info
						color='info'
						fontSize='small'
					/>
				</Tooltip>
			</Box>

			<FormProvider {...form}>
				<form
					id='checkout-form'
					onSubmit={onSubmit}>
					<Stack spacing={2}>
						<TextInput
							id='fullName'
							label='Full Name'
							disabled
							fullWidth
						/>

						<TextInput
							id='email'
							label='Email'
							disabled
							fullWidth
						/>

						<TextInput
							id='city'
							label='City'
							required
							fullWidth
						/>

						<TextInput
							id='street'
							label='Street'
							required
							fullWidth
						/>

						<TextInput
							id='houseNumber'
							label='House Number'
							required
							fullWidth
						/>
					</Stack>
				</form>
			</FormProvider>
		</Box>
	);
}

import {
	Box,
	Button,
	Divider,
	Drawer,
	Stack,
	Toolbar,
	Typography,
} from '@mui/material';
import { ArrowUpward } from '@mui/icons-material';
import type { ProductResponse } from '../../api/openApi/model';
import useEditProductLogic from '../../hooks/pageLogic/admin/useEditProductLogic.ts';
import { useEffect } from 'react';
import { useSnackbar } from 'notistack';
import { FormProvider } from 'react-hook-form';
import TextInput from '../../shared/components/ui/textInput.tsx';
import CircularProgress from '@mui/material/CircularProgress';
import EditIcon from '@mui/icons-material/Edit';

type Props = {
	open: boolean;
	handleClose: () => void;
	product: ProductResponse | null;
	page: number;
	pageSize: number;
};

export default function EditProductDrawer({
	open,
	handleClose,
	product,
	page,
	pageSize,
}: Props) {
	const {
		form,
		submitEditForm,
		isSuccess,
		isError,
		errorMessage,
		isLoading,
		allSet,
		setAllSet,
	} = useEditProductLogic(product, page, pageSize);
	const { enqueueSnackbar } = useSnackbar();

	useEffect(() => {
		if (!isError) return;

		enqueueSnackbar(errorMessage, { variant: 'error' });
	}, [isError, errorMessage, enqueueSnackbar]);

	useEffect(() => {
		if (!isSuccess) return;

		enqueueSnackbar('Successfully updated product', { variant: 'success' });
		handleClose();
	}, [isSuccess, enqueueSnackbar]);

	useEffect(() => {
		if (!allSet) return;

		enqueueSnackbar('All set - there is noting to update.', {
			variant: 'info',
		});
		setAllSet(false);
		handleClose();
	}, [allSet, handleClose, enqueueSnackbar, setAllSet]);

	return (
		<Drawer
			open={open}
			onClose={handleClose}
			anchor='right'
			variant='temporary'>
			<Box width='450px'>
				<Toolbar
					sx={{
						display: 'flex',
						alignItems: 'center',
						gap: 3,
					}}>
					<EditIcon
						color='primary'
						fontSize='large'
					/>
					<Typography
						variant='h6'
						color='primary'
						fontWeight={700}>
						Edit "{product?.name}"
					</Typography>
				</Toolbar>
				<Divider />

				<Box
					mt={3}
					px={3}>
					<FormProvider {...form}>
						<form
							id='edit-product-form'
							onSubmit={submitEditForm}>
							<Stack spacing={3}>
								<TextInput
									id='name'
									label='Name'
									required
									fullWidth
								/>

								<TextInput
									id='category'
									label='Category'
									required
									fullWidth
								/>

								<TextInput
									id='price'
									label='Price'
									type='number'
									required
									fullWidth
								/>

								<Button
									startIcon={<ArrowUpward />}
									form='edit-product-form'
									type='submit'
									loading={isLoading}
									loadingIndicator={<CircularProgress size={26} />}
									color='primary'
									sx={{
										height: 50,
										fontWeight: 700,
										fontSize: 15,
									}}>
									Submit Changes
								</Button>
							</Stack>
						</form>
					</FormProvider>
				</Box>
			</Box>
		</Drawer>
	);
}

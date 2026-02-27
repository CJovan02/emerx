import { useCreateProductLogic } from '../../hooks/pageLogic/admin/useCreateProductLogic.ts';
import { useEffect } from 'react';
import { useSnackbar } from 'notistack';
import {
	Box,
	Button,
	Divider,
	Drawer,
	Stack,
	Toolbar,
	Typography,
} from '@mui/material';
import { AddCircleOutlineOutlined, ArrowUpward } from '@mui/icons-material';
import { FormProvider } from 'react-hook-form';
import TextInput from '../../shared/components/ui/textInput.tsx';
import CircularProgress from '@mui/material/CircularProgress';
import { ProductImageDropzoneInput } from '../../shared/components/productImageDropzoneInput.tsx';

type Props = {
	open: boolean;
	handleClose: () => void;
	page: number;
	pageSize: number;
};

export default function CreateProductDrawer({
	open,
	handleClose,
	pageSize,
	page,
}: Props) {
	const {
		form,
		isError,
		errorMessage,
		isLoading,
		submitCreateForm,
		isSuccess,
	} = useCreateProductLogic(page, pageSize);
	const { enqueueSnackbar } = useSnackbar();

	useEffect(() => {
		if (!isError) return;

		enqueueSnackbar(errorMessage, { variant: 'error' });
	}, [isError, errorMessage, enqueueSnackbar]);

	useEffect(() => {
		if (!isSuccess) return;

		form.reset();
		enqueueSnackbar('Successfully created product', { variant: 'success' });
		//handleClose();
	}, [isSuccess, enqueueSnackbar, form.reset]);

	return (
		<Drawer
			open={open}
			onClose={handleClose}
			anchor='right'
			variant='temporary'>
			<Box
				width='500px'
				pb={3}>
				<Toolbar
					sx={{
						display: 'flex',
						alignItems: 'center',
						gap: 3,
					}}>
					<AddCircleOutlineOutlined
						color='primary'
						fontSize='large'
					/>
					<Typography
						variant='h6'
						color='primary'
						fontWeight={700}>
						Create New Product
					</Typography>
				</Toolbar>
				<Divider />

				<Box
					mt={3}
					px={3}>
					<FormProvider {...form}>
						<form
							id='create-product-form'
							onSubmit={submitCreateForm}>
							<Stack spacing={3}>
								<ProductImageDropzoneInput id='image' />

								<TextInput
									id='name'
									label='Name'
									required
									fullWidth
								/>

								<TextInput
									id='description'
									label='Description'
									minRows={3}
									maxRows={10}
									multiline
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

								<TextInput
									id='stock'
									label='Stock'
									type='number'
									required
									fullWidth
								/>

								<Button
									startIcon={<ArrowUpward />}
									form='create-product-form'
									type='submit'
									loading={isLoading}
									loadingIndicator={<CircularProgress size={26} />}
									color='primary'
									sx={{
										height: 50,
										fontWeight: 700,
										fontSize: 15,
									}}>
									Create Product
								</Button>
							</Stack>
						</form>
					</FormProvider>
				</Box>
			</Box>
		</Drawer>
	);
}

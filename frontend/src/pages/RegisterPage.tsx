import {
	Box,
	Button,
	Card,
	CardActions,
	CardContent,
	CardHeader,
	Divider,
	Link,
	Stack,
} from '@mui/material';
import { FormProvider } from 'react-hook-form';
import TextInput from '../componenets/reusable/textInput';
import useRegisterLogic from '../hooks/useRegisterLogic';
import { Routes } from '../shared/common/constants/routeNames';
import { Link as RouterLink, useNavigate } from 'react-router';
import { useSnackbar } from 'notistack';
import { useEffect } from 'react';

export default function RegisterPage() {
	const {
		form,
		handleFormRegister,
		isError,
		isLoading,
		isSuccess,
		errorMessage,
	} = useRegisterLogic();
	const { enqueueSnackbar } = useSnackbar();
	const navigate = useNavigate();

	useEffect(() => {
		if (!isError || !errorMessage) return;

		enqueueSnackbar(errorMessage, { variant: 'error' });
	}, [isError, errorMessage]);
	useEffect(() => {
		if (!isSuccess) return;

		enqueueSnackbar(
			'You successfully created a new account. Please log in with it to start shopping!',
			{ variant: 'success' }
		);
		navigate(Routes.Login, { replace: true });
	}, [isSuccess]);

	return (
		<Box
			display='flex'
			justifyContent='center'
			alignItems='center'
			minHeight='100vh'
			sx={{
				background: theme => theme.custom.gradients.background,
			}}>
			<Card
				elevation={3}
				sx={{ padding: 2 }}>
				<CardHeader
					title='Sign Up'
					subheader='Create a new account to explore our products.'
				/>
				<CardContent>
					<FormProvider {...form}>
						<form
							id='register-form'
							onSubmit={handleFormRegister}>
							<Stack gap={3}>
								<TextInput
									id='name'
									label='Name'
									required
									fullWidth
								/>

								<TextInput
									id='surname'
									label='Surname'
									required
									fullWidth
								/>

								<TextInput
									id='email'
									label='Email'
									required
									fullWidth
								/>

								<TextInput
									id='password'
									label='Password'
									type='password'
									required
									fullWidth
								/>
							</Stack>
						</form>
					</FormProvider>
				</CardContent>
				<CardActions>
					<Button
						type='submit'
						form='register-form'
						fullWidth
						size='large'
						variant='contained'
						sx={{
							py: 1.5,
						}}
						loading={isLoading}>
						Sign In
					</Button>
				</CardActions>
				<CardContent>
					<Divider />
					<Box marginY={2}>
						Already have an account?{' '}
						<Link
							component={RouterLink}
							to={Routes.Login}
							replace>
							Sign In
						</Link>
					</Box>
				</CardContent>
			</Card>
		</Box>
	);
}

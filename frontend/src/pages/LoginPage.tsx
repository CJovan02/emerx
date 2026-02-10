import {
	Alert,
	Box,
	Button,
	Card,
	CardActions,
	CardContent,
	CardHeader,
	Divider,
	Link,
	Snackbar,
	Stack,
} from '@mui/material';
import TextInput from '../componenets/reusable/textInput.tsx';
import useLoginLogic from '../hooks/useLoginLogic.ts';
import { FormProvider } from 'react-hook-form';
import { useEffect, useState } from 'react';
import { Link as RouterLink } from 'react-router';
import { Routes } from '../shared/common/constants/routeNames.ts';

const LoginPage = () => {
	const { form, login, isLoading, isError, errorMessage } = useLoginLogic();
	const [showErrorSnackbar, setShowErrorSnackbar] = useState(false);

	useEffect(() => {
		if (isError) setShowErrorSnackbar(true);
	}, [isError]);

	return (
		<Box
			minHeight='100vh'
			display='flex'
			alignItems='center'
			justifyContent='center'
			sx={{
				background: theme => theme.custom.gradients.background,
			}}>
			<Snackbar
				open={showErrorSnackbar}
				autoHideDuration={4000}
				onClose={() => setShowErrorSnackbar(false)}>
				<Alert
					severity='error'
					variant='filled'
					sx={{ width: '100%' }}>
					{errorMessage}
				</Alert>
			</Snackbar>
			<Card
				elevation={3}
				sx={{ padding: 2 }}>
				<CardHeader
					title='Sign In'
					subheader='Welcome back, please login to start shopping'
				/>
				<CardContent>
					<FormProvider {...form}>
						<form
							id='login-form'
							onSubmit={form.handleSubmit(login)}>
							<Stack gap={3}>
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
						form='login-form'
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
						Don't have an account?{' '}
						<Link
							component={RouterLink}
							to={Routes.Register}>
							Sign up
						</Link>
					</Box>
				</CardContent>
			</Card>
		</Box>
	);
};

export default LoginPage;

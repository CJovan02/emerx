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
	Stack,
} from '@mui/material';
import TextInput from '../componenets/reusable/textInput.tsx';
import useLoginLogic from '../hooks/useLoginLogic.ts';
import { FormProvider } from 'react-hook-form';
import { useEffect } from 'react';
import { Link as RouterLink } from 'react-router';
import { Routes } from '../shared/common/constants/routeNames.ts';
import { useSnackbar } from 'notistack';

const LoginPage = () => {
	const { form, login, isLoading, isError, errorMessage } = useLoginLogic();
	const { enqueueSnackbar } = useSnackbar();

	useEffect(() => {
		if (!isError) return;

		enqueueSnackbar(errorMessage, { variant: 'error' });
	}, [isError, errorMessage]);

	return (
		<Box
			minHeight='100vh'
			display='flex'
			alignItems='center'
			justifyContent='center'
			sx={{
				background: theme => theme.custom.gradients.background,
			}}>
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
							to={Routes.Register}
							replace>
							Sign up
						</Link>
					</Box>
				</CardContent>
			</Card>
		</Box>
	);
};

export default LoginPage;

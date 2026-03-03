import { EditNotifications } from '@mui/icons-material';
import {
	Avatar,
	Box,
	Button,
	Card,
	CardContent,
	CircularProgress,
	Container,
	Divider,
	Stack,
	TextField,
	Typography,
} from '@mui/material';
import { useEffect, useState } from 'react';
import { useUserStore } from '../stores/userStore';
import type { UpdateUserRequest } from '../api/openApi/model';
import { useForm, Controller } from 'react-hook-form';
import { useUserUpdate } from '../api/openApi/user/user';
import AppUser from '../domain/models/appUser';

export default function MyProfilePage() {
	const { user, setUser } = useUserStore();
	const [isEditing, setIsEditing] = useState(false);
	const { mutate: updateUser, isPending: isLoading } = useUserUpdate();

	const updateAppUserValues = (updatedUser: UpdateUserRequest) => {
		if (!user) return;

		const updatedAppUser = new AppUser(
			user.id,
			updatedUser.name,
			updatedUser.surname,
			user.email, // email not being updated
			updatedUser.address ?? user.address,
			user.roles
		);

		setUser(updatedAppUser);
	};
	console.log(user);

	const { control, handleSubmit, reset } = useForm<UpdateUserRequest>({
		defaultValues: {
			name: user?.name || '',
			surname: user?.surname || '',
			address: {
				city: user?.address?.city || '',
				street: user?.address?.street || '',
				houseNumber: user?.address?.houseNumber || '',
			},
		},
	});

	useEffect(() => {
		if (user) {
			reset(user);
		}
	}, [user, reset]);

	if (!user) return null;

	const onSubmit = (data: UpdateUserRequest) => {
		updateUser(
			{ id: user.id, data },
			{
				onSuccess: updatedUser => {
					updateAppUserValues(updatedUser);
					setIsEditing(false);
				},
				onError: error => {
					console.error('Failed to update user:', error);
				},
			}
		);
	};

	const handleEdit = () => {
		reset(user);
		setIsEditing(true);
	};

	const handleCancel = () => {
		reset(user);
		setIsEditing(false);
	};

	return (
		<Container
			maxWidth='md'
			sx={{ mt: 6 }}>
			<Card
				sx={{
					borderRadius: 4,
					boxShadow: 4,
					position: 'relative',
				}}>
				<CardContent sx={{ opacity: isLoading ? 0.5 : 1 }}>
					<Stack spacing={4}>
						<Stack
							direction='row'
							spacing={3}
							alignItems='flex-start'>
							<Avatar sx={{ width: 100, height: 100, fontSize: 40 }}>
								{user.name?.[0] || 'U'}
							</Avatar>

							<Box flex={1}>
								{!isEditing ? (
									<>
										<Typography variant='h5'>
											{user.name} {user.surname}
										</Typography>
										<Typography color='text.secondary'>{user.email}</Typography>

										<Box mt={2}>
											<Typography variant='h6'>Address</Typography>
											<Typography color='text.secondary'>
												<b>City:</b> {user.address?.city || 'Not provided'}
											</Typography>
											<Typography color='text.secondary'>
												<b>Street:</b> {user.address?.street || 'Not provided'}
											</Typography>
											<Typography color='text.secondary'>
												<b>House number:</b>{' '}
												{user.address?.houseNumber || 'Not provided'}
											</Typography>
										</Box>
									</>
								) : (
									<form onSubmit={handleSubmit(onSubmit)}>
										<Stack spacing={2}>
											<Stack
												direction='row'
												spacing={2}>
												<Controller
													name='name'
													control={control}
													render={({ field }) => (
														<TextField
															{...field}
															label='Name'
															fullWidth
														/>
													)}
												/>
												<Controller
													name='surname'
													control={control}
													render={({ field }) => (
														<TextField
															{...field}
															label='Surname'
															fullWidth
														/>
													)}
												/>
											</Stack>

											<Typography variant='h6'>Address</Typography>

											<Stack
												direction='row'
												spacing={2}>
												<Controller
													name='address.city'
													control={control}
													render={({ field }) => (
														<TextField
															{...field}
															label='City'
															fullWidth
														/>
													)}
												/>
												<Controller
													name='address.street'
													control={control}
													render={({ field }) => (
														<TextField
															{...field}
															label='Street'
															fullWidth
														/>
													)}
												/>
												<Controller
													name='address.houseNumber'
													control={control}
													render={({ field }) => (
														<TextField
															{...field}
															label='House Number'
															fullWidth
														/>
													)}
												/>
											</Stack>

											<Stack
												direction='row'
												spacing={2}
												justifyContent='flex-end'>
												<Button
													variant='outlined'
													onClick={handleCancel}>
													Cancel
												</Button>
												<Button
													type='submit'
													variant='contained'
													disabled={isLoading}>
													Save
												</Button>
											</Stack>
										</Stack>
									</form>
								)}
							</Box>

							{!isEditing && (
								<Button
									variant='contained'
									startIcon={<EditNotifications />}
									onClick={handleEdit}>
									Edit
								</Button>
							)}
						</Stack>
						<Divider />
					</Stack>
				</CardContent>

				{/* Spinner Overlay */}
				{isLoading && (
					<Box
						sx={{
							position: 'absolute',
							inset: 0,
							display: 'flex',
							alignItems: 'center',
							justifyContent: 'center',
						}}>
						<CircularProgress />
					</Box>
				)}
			</Card>
		</Container>
	);
}

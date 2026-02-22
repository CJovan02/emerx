import { EditNotifications } from '@mui/icons-material';
import {
	Avatar,
	Box,
	Button,
	Card,
	CardContent,
	Container,
	Divider,
	Stack,
	TextField,
	Typography,
} from '@mui/material';
import { useState } from 'react';
import { useUserStore } from '../stores/userStore';
import type { UserResponse } from '../api/openApi/model';
import { useForm, Controller } from 'react-hook-form';

export default function MyProfilePage() {
	const { user, setUser } = useUserStore();
	const [isEditing, setIsEditing] = useState(false);

	const { control, handleSubmit, reset } = useForm<UserResponse>({
		defaultValues: user || undefined,
	});

	if (!user) return null;

	const onSubmit = (data: UserResponse) => {
		setUser(data);
		setIsEditing(false);
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
			<Card sx={{ borderRadius: 4, boxShadow: 4 }}>
				<CardContent>
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
												<b>House number:</b>
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
													variant='contained'>
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
			</Card>
		</Container>
	);
}

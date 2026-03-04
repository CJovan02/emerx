import {
	Alert,
	Box,
	Button,
	Card,
	CardContent,
	CircularProgress,
	Rating,
	Stack,
	TextField,
	Typography,
} from '@mui/material';
import { Controller } from 'react-hook-form';
import useProductReviewsLogic from '../../hooks/pageLogic/useProductReviewsLogic.ts';
import ProductReviewCard from './ProductReviewCard.tsx';

type Props = {
	productId: string;
};

export default function ProductReviewsSection({ productId }: Props) {
	const {
		reviews,
		isLoading,
		form,
		submitReview,
		isSubmitting,
		userHasReviewed,
		user,
		submitError,
		editingReviewId,
		editForm,
		isEditSubmitting,
		editError,
		startEditing,
		cancelEditing,
		submitEdit,
	} = useProductReviewsLogic(productId);

	const { register, control, formState: { errors } } = form;
	const {
		register: editRegister,
		control: editControl,
		formState: { errors: editErrors },
	} = editForm;

	return (
		<Box>
			<Typography
				variant='h6'
				fontWeight={600}
				mb={2}>
				Customer Reviews
			</Typography>

			{!userHasReviewed && user && (
				<Box
					component='form'
					onSubmit={submitReview}
					mb={4}>
					<Typography
						variant='subtitle1'
						fontWeight={500}
						mb={1}>
						Write a Review
					</Typography>

					<Box mb={1}>
						<Controller
							name='rating'
							control={control}
							render={({ field }) => (
								<Rating
									value={field.value}
									onChange={(_, value) => field.onChange(value ?? 0)}
								/>
							)}
						/>
						{errors.rating && (
							<Typography
								variant='caption'
								color='error'
								display='block'>
								{errors.rating.message}
							</Typography>
						)}
					</Box>

					<TextField
						{...register('description')}
						label='Description'
						multiline
						rows={3}
						fullWidth
						error={!!errors.description}
						helperText={errors.description?.message}
						sx={{ mb: 2 }}
					/>

					{submitError && (
						<Alert
							severity='error'
							sx={{ mb: 2 }}>
							{submitError}
						</Alert>
					)}

					<Button
						type='submit'
						variant='contained'
						loading={isSubmitting}
						disabled={isSubmitting}>
						Submit Review
					</Button>
				</Box>
			)}

			{isLoading ? (
				<Box
					display='flex'
					justifyContent='center'>
					<CircularProgress size={32} />
				</Box>
			) : !reviews || reviews.length === 0 ? (
				<Typography
					variant='body2'
					color='text.secondary'>
					No reviews yet.
				</Typography>
			) : (
				<Stack spacing={2}>
					{reviews.map(review => {
						const reviewId = review.id as unknown as string;
						const isEditing = editingReviewId === reviewId;

						if (isEditing) {
							return (
								<Card
									key={reviewId}
									variant='outlined'>
									<CardContent>
										<Typography
											variant='subtitle2'
											fontWeight={600}
											mb={1}>
											Edit Your Review
										</Typography>

										<Box
											component='form'
											onSubmit={submitEdit}>
											<Box mb={1}>
												<Controller
													name='rating'
													control={editControl}
													render={({ field }) => (
														<Rating
															value={field.value}
															onChange={(_, value) =>
																field.onChange(value ?? 0)
															}
														/>
													)}
												/>
												{editErrors.rating && (
													<Typography
														variant='caption'
														color='error'
														display='block'>
														{editErrors.rating.message}
													</Typography>
												)}
											</Box>

											<TextField
												{...editRegister('description')}
												label='Description'
												multiline
												rows={3}
												fullWidth
												error={!!editErrors.description}
												helperText={editErrors.description?.message}
												sx={{ mb: 2 }}
											/>

											{editError && (
												<Alert
													severity='error'
													sx={{ mb: 2 }}>
													{editError}
												</Alert>
											)}

											<Box
												display='flex'
												gap={1}>
												<Button
													type='submit'
													variant='contained'
													loading={isEditSubmitting}
													disabled={isEditSubmitting}>
													Save
												</Button>
												<Button
													variant='outlined'
													disabled={isEditSubmitting}
													onClick={cancelEditing}>
													Cancel
												</Button>
											</Box>
										</Box>
									</CardContent>
								</Card>
							);
						}

						return (
							<ProductReviewCard
								key={reviewId}
								review={review}
								isOwnReview={
									(review.userId as unknown as string) === user?.id
								}
								onEditClick={() => startEditing(review)}
							/>
						);
					})}
				</Stack>
			)}
		</Box>
	);
}

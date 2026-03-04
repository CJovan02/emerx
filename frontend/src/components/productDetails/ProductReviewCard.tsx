import {
	Avatar,
	Box,
	Button,
	Card,
	CardContent,
	Chip,
	Rating,
	Stack,
	Typography,
} from '@mui/material';
import EditIcon from '@mui/icons-material/Edit';
import type { ReviewResponse } from '../../api/openApi/model';

type Props = {
	review: ReviewResponse;
	isOwnReview: boolean;
	onEditClick?: () => void;
};

export default function ProductReviewCard({
	review,
	isOwnReview,
	onEditClick,
}: Props) {
	return (
		<Card variant='outlined'>
			<CardContent>
				<Stack spacing={2}>
					{/* Own review indicator + edit */}
					{isOwnReview && (
						<Stack
							direction='row'
							justifyContent='space-between'
							alignItems='center'>
							<Chip
								label='Your review'
								size='small'
								color='primary'
								variant='outlined'
							/>

							<Button
								size='small'
								startIcon={<EditIcon />}
								onClick={onEditClick}>
								Edit
							</Button>
						</Stack>
					)}

					{/* Rating */}
					<Rating
						value={review.rating}
						precision={0.5}
						readOnly
						size='large'
					/>

					{/* Description */}
					<Typography
						variant='body1'
						sx={{
							fontStyle: 'italic',
							lineHeight: 1.7,
						}}>
						“{review.description}”
					</Typography>

					<Box mt={2} />

					{/* User info */}
					<Stack
						direction='row'
						spacing={1.5}
						alignItems='center'>
						<Avatar
							sx={{
								width: 40,
								height: 40,
								fontWeight: 600,
							}}>
							{review.userFullName?.[0]}
						</Avatar>

						<Typography
							variant='subtitle2'
							fontWeight={600}>
							{review.userFullName}
						</Typography>
					</Stack>
				</Stack>
			</CardContent>
		</Card>
	);
}

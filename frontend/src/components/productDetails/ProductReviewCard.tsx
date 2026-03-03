import { Box, Button, Card, CardContent, Chip, Rating, Typography } from '@mui/material';
import type { ReviewResponse } from '../../api/openApi/model';

type Props = {
	review: ReviewResponse;
	isOwnReview: boolean;
	onEditClick?: () => void;
};

export default function ProductReviewCard({ review, isOwnReview, onEditClick }: Props) {
	return (
		<Card variant='outlined'>
			<CardContent>
				<Box
					display='flex'
					alignItems='center'
					gap={1}
					mb={1}>
					<Rating
						value={review.rating}
						precision={0.1}
						size='small'
						readOnly
					/>
					{isOwnReview && (
						<>
							<Chip
								label='Your review'
								size='small'
								color='primary'
								variant='outlined'
							/>
							<Button
								size='small'
								onClick={onEditClick}>
								Edit
							</Button>
						</>
					)}
				</Box>
				<Typography variant='body2'>{review.description}</Typography>
			</CardContent>
		</Card>
	);
}

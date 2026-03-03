import z from 'zod';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { useMemo, useState } from 'react';
import { useQueryClient } from '@tanstack/react-query';
import { isAxiosError } from 'axios';
import {
	useReviewGetByProductId,
	useReviewCreate,
	useReviewPatch,
	getReviewGetByProductIdQueryKey,
} from '../../api/openApi/review/review.ts';
import { getProductGetByIdQueryKey } from '../../api/openApi/product/product.ts';
import { useUserStore } from '../../stores/userStore.ts';
import type { ProblemDetails, ReviewResponse } from '../../api/openApi/model';
import type { ErrorType } from '../../api/axiosInstance.ts';

const reviewFormSchema = z.object({
	rating: z.number().min(1, 'Please select a rating').max(5),
	description: z.string().min(1, 'Description is required'),
});

type FormValues = z.infer<typeof reviewFormSchema>;

function useProductReviewsLogic(productId: string) {
	const user = useUserStore(state => state.user);
	const queryClient = useQueryClient();

	const { data: reviews, isPending: isLoading } =
		useReviewGetByProductId(productId);

	const userHasReviewed = useMemo(() => {
		if (!reviews || !user) return false;
		return reviews.some(
			review => (review.userId as unknown as string) === user.id
		);
	}, [reviews, user]);

	// --- Create form ---
	const form = useForm<FormValues>({
		resolver: zodResolver(reviewFormSchema),
		defaultValues: { rating: 0, description: '' },
	});

	const [submitError, setSubmitError] = useState<string | null>(null);

	const { mutateAsync: createAsync, isPending: isSubmitting } = useReviewCreate({
		mutation: {
			onSuccess: () => {
				queryClient.invalidateQueries({
					queryKey: getReviewGetByProductIdQueryKey(productId),
				});
				queryClient.invalidateQueries({
					queryKey: getProductGetByIdQueryKey(productId),
				});
				form.reset();
				setSubmitError(null);
			},
			onError: (err) => handleMutationError(err, setSubmitError),
		},
	});

	const submitReview = form.handleSubmit(async ({ rating, description }: FormValues) => {
		if (!user) return;
		await createAsync({
			data: { userId: user.id, productId, rating, description },
		});
	});

	// --- Edit form ---
	const [editingReviewId, setEditingReviewId] = useState<string | null>(null);
	const [editError, setEditError] = useState<string | null>(null);

	const editForm = useForm<FormValues>({
		resolver: zodResolver(reviewFormSchema),
		defaultValues: { rating: 0, description: '' },
	});

	const { mutateAsync: patchAsync, isPending: isEditSubmitting } = useReviewPatch({
		mutation: {
			onSuccess: () => {
				queryClient.invalidateQueries({
					queryKey: getReviewGetByProductIdQueryKey(productId),
				});
				queryClient.invalidateQueries({
					queryKey: getProductGetByIdQueryKey(productId),
				});
				setEditingReviewId(null);
				setEditError(null);
			},
			onError: (err) => handleMutationError(err, setEditError),
		},
	});

	function startEditing(review: ReviewResponse) {
		setEditingReviewId(review.id as unknown as string);
		setEditError(null);
		editForm.reset({ rating: review.rating, description: review.description });
	}

	function cancelEditing() {
		setEditingReviewId(null);
		setEditError(null);
	}

	const submitEdit = editForm.handleSubmit(async ({ rating, description }: FormValues) => {
		if (!editingReviewId) return;
		await patchAsync({
			id: editingReviewId,
			data: { rating, description },
		});
	});

	return {
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
	};
}

function handleMutationError(
	error: ErrorType<void | ProblemDetails>,
	setError: (msg: string | null) => void
) {
	if (!error) {
		setError(null);
		return;
	}

	if (isAxiosError(error) && error.response?.status === 400) {
		setError('You must have purchased this product before you can review it.');
		return;
	}

	setError('Unexpected error occurred, please try again.');
}

export default useProductReviewsLogic;

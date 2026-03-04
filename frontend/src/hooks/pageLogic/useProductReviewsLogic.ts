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
import type { ReviewResponse } from '../../api/openApi/model';
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

		return reviews.some(review => review.userId === user.id);
	}, [reviews, user]);

	// --- Create form ---
	const form = useForm<FormValues>({
		resolver: zodResolver(reviewFormSchema),
		defaultValues: { rating: 0, description: '' },
	});

	const {
		mutateAsync: createAsync,
		isPending: isSubmitting,
		isError: createIsError,
		error: createError,
	} = useReviewCreate({
		mutation: {
			onSuccess: () => {
				queryClient.invalidateQueries({
					queryKey: getReviewGetByProductIdQueryKey(productId),
				});
				queryClient.invalidateQueries({
					queryKey: getProductGetByIdQueryKey(productId),
				});
				form.reset();
			},
		},
	});
	const createErrorMessage: string | null = useMemo(() => {
		if (!createIsError) return null;

		if (isAxiosError(createError)) {
			switch (createError.response?.status) {
				case 403:
					return 'You need to order this product before leaving a review.';
				case 409:
					return 'You already posted a review for this product.';
			}
		}

		return 'Unexpected error occurred while trying to post your review, please try again.';
	}, [createIsError, createError]);

	const submitReview = form.handleSubmit(
		async ({ rating, description }: FormValues) => {
			if (!user) return;
			await createAsync({
				data: { productId, rating, description },
			});
		}
	);

	// --- Edit form ---
	const [editingReviewId, setEditingReviewId] = useState<string | null>(null);
	const [editError, setEditError] = useState<string | null>(null);

	const editForm = useForm<FormValues>({
		resolver: zodResolver(reviewFormSchema),
		defaultValues: { rating: 0, description: '' },
	});

	const {
		mutateAsync: patchAsync,
		isPending: isEditSubmitting,
		isError: patchIsError,
		error: patchError,
	} = useReviewPatch({
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
		},
	});
	const patchErrorMessage: string | null = useMemo(() => {
		if (!patchIsError) return null;
		return 'Unexpected error happened while trying to edit your review, please try again.';
	}, [patchIsError, patchError]);

	function startEditing(review: ReviewResponse) {
		setEditingReviewId(review.id as unknown as string);
		setEditError(null);
		editForm.reset({ rating: review.rating, description: review.description });
	}

	function cancelEditing() {
		setEditingReviewId(null);
		setEditError(null);
	}

	const submitEdit = editForm.handleSubmit(
		async ({ rating, description }: FormValues) => {
			if (!editingReviewId) return;
			await patchAsync({
				id: editingReviewId,
				data: { rating, description },
			});
		}
	);

	return {
		reviews,
		isLoading,
		form,
		submitReview,
		isSubmitting,
		userHasReviewed,
		user,
		createErrorMessage,
		patchErrorMessage,
		editingReviewId,
		editForm,
		isEditSubmitting,
		editError,
		startEditing,
		cancelEditing,
		submitEdit,
	};
}

export default useProductReviewsLogic;

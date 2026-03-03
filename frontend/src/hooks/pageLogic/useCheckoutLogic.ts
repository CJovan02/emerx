import { useMemo, useState } from 'react';
import { useForm } from 'react-hook-form';
import z from 'zod';
import { zodResolver } from '@hookform/resolvers/zod';
import { useUserStore } from '../../stores/userStore.ts';
import { useCartStore } from '../../stores/cartStore.ts';
import { useOrderReview } from '../../api/openApi/order/order.ts';
import type { OrderItemRequest } from '../../api/openApi/model';
import { isAxiosError } from 'axios';

type Stage = 'form' | 'review';

export default function useCheckoutLogic() {
	const [stage, setStage] = useState<Stage>('form');

	const userFullName = useUserStore(
		state => state.user!.name + ' ' + state.user!.surname
	);
	const userEmail = useUserStore(state => state.user!.email);
	const userAddress = useUserStore(state => state.user!.address);

	const cartItems = useCartStore(state => state.items);

	// form
	const FormSchema = z.object({
		fullName: z.string().nonempty('Required'),

		email: z.email().nonempty('Required'),

		city: z
			.string()
			.min(3, 'City must be at least 3 characters')
			.max(50, "City can't be larger than 50 characters"),

		street: z
			.string()
			.min(3, 'Street must be at least 3 characters')
			.max(50, "Street can't be larger than 50 characters"),

		houseNumber: z
			.string()
			.min(1, 'House number must be at least 1 characters')
			.max(5, "House number can't be larger than 5 characters"),
	});
	type FormValues = z.infer<typeof FormSchema>;

	const form = useForm({
		resolver: zodResolver(FormSchema),
		defaultValues: {
			fullName: userFullName,
			email: userEmail,
			city: userAddress?.city ?? '',
			street: userAddress?.street ?? '',
			houseNumber: userAddress?.houseNumber ?? '',
		},
	});

	// react query
	const reviewMutation = useOrderReview();
	const {
		data: reviewData,
		isPending: reviewIsPending,
		isError: reviewIsError,
		error: reviewError,
		mutateAsync: reviewMutateAsync,
	} = reviewMutation;
	const reviewErrorMessage: string | null = useMemo(() => {
		if (!reviewIsError) return null;

		if (isAxiosError(reviewError)) {
			// These are not handled great, a lot of UI error handling needs to be done here, but this will do
			switch (reviewError.response?.status) {
				case 404:
					return "Some of the products you are trying to order don't exist anymore";
				case 409:
					return 'Quantities of some products that you are trying to order are not available anymore';
			}
		}

		return 'Unexpected error happened, please try again';
	}, [reviewIsError, reviewError]);

	// business logic
	const handleFormContinueToReview = form.handleSubmit(
		async (_: FormValues) => {
			setStage('review');
			const requestOrderItems = cartItems.map(i => {
				return {
					productId: i.productId,
					quantity: i.quantity,
				} as OrderItemRequest;
			});
			await reviewMutateAsync({ data: { items: requestOrderItems } });
		}
	);
	function goBackToForm() {
		setStage('form');
	}

	return {
		form,
		stage,
		isFormStage: stage === 'form',
		isReviewStage: stage === 'review',
		cartItems,
		reviewErrorMessage,
		reviewData,
		reviewIsPending,
		reviewIsError,
		handleFormContinueToReview,
		goBackToForm,
	};
}

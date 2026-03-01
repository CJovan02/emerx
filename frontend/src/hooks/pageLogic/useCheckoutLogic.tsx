import { useState } from 'react';
import { useForm } from 'react-hook-form';
import z from 'zod';
import { zodResolver } from '@hookform/resolvers/zod';
import { useUserStore } from '../../stores/userStore.ts';
import { useCartStore } from '../../stores/cartStore.ts';

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

	// business logic
	const handleFormContinueToReview = form.handleSubmit((data: FormValues) => {
		console.log(data);
		setStage('review');
	});
	function goBackToForm() {
		setStage('form');
	}

	return {
		form,
		stage,
		isFormStage: stage === 'form',
		isReviewStage: stage === 'review',
		cartItems,
		handleFormContinueToReview,
		goBackToForm,
	};
}

import z from 'zod';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { useQueryClient } from '@tanstack/react-query';
import { useProductCreate } from '../../../api/openApi/product/product.ts';
import { QueryKeys } from '../../../shared/common/queryKeys.ts';
import { useState } from 'react';
import type {
	ProblemDetails,
	ProductCreateBody,
} from '../../../api/openApi/model';
import type { ErrorType } from '../../../api/axiosInstance.ts';
import { isAxiosError } from 'axios';

export function useCreateProductLogic(page: number, pageSize: number) {
	// form
	const formSchema = z.object({
		name: z
			.string()
			.nonempty("Name is required")
			.min(3, 'Name must be at least 3 characters.')
			.max(30, "Name can't be larger than 30 characters."),

		description: z
			.string()
			.nonempty("Description is required")
			.min(10, 'Description must be at least 3 characters.')
			.max(300, "Description can't be larger than 300 characters."),

		category: z
			.string()
			.nonempty()
			.min(3, 'Category must be at least 3 characters.')
			.max(30, "Category can't be larger than 30 characters."),

		price: z.number().nonnegative(),

		stock: z.number().nonnegative(),

		image: z
			.file()
			.optional()
			.nullable()
			.transform(val => val ?? undefined),
	});

	type FormValues = z.infer<typeof formSchema>;

	const form = useForm({
		resolver: zodResolver(formSchema),
	});

	// api functions
	const queryClient = useQueryClient();
	const { mutateAsync, isPending, isError, isSuccess } = useProductCreate({
		mutation: {
			onError: handleError,
			// When we create product, we refetch the page that contained that product
			onSuccess: () => {
				queryClient.invalidateQueries({
					queryKey: QueryKeys.adminGetProductsPaged(page, pageSize),
				});
			},
		},
	});
	const [errorMessage, setErrorMessage] = useState<string | null>(null);

	const submitCreateForm = form.handleSubmit(
		async ({
			name,
			category,
			price,
			image,
			stock,
			description,
		}: FormValues) => {
			const request: ProductCreateBody = {
				Description: description,
				Stock: stock,
				Name: name,
				Category: category,
				Price: price,
				Image: image,
			};

			await mutateAsync({ data: request });
		}
	);

	function handleError(error: ErrorType<void | ProblemDetails>) {
		if (!error) {
			setErrorMessage(null);
			return;
		}

		if (isAxiosError(error)) {
			switch (error.response?.status) {
				case 400:
					setErrorMessage(
						'You entered wrong input for the product, change form values and try again.'
					);
					return;
			}
		}

		setErrorMessage('Unexpected error occurred, please try again.');
	}

	return {
		form,
		isSuccess,
		isError,
		errorMessage,
		isLoading: isPending,
		submitCreateForm,
	};
}

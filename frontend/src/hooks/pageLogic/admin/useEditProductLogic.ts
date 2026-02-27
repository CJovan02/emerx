import { useCallback, useEffect, useState } from 'react';
import type {
	ProblemDetails,
	ProductPatchBody,
	ProductResponse,
} from '../../../api/openApi/model';
import { useForm } from 'react-hook-form';
import z from 'zod';
import { zodResolver } from '@hookform/resolvers/zod';
import { useProductPatch } from '../../../api/openApi/product/product.ts';
import type { ErrorType } from '../../../api/axiosInstance.ts';
import { isAxiosError } from 'axios';
import { useQueryClient } from '@tanstack/react-query';
import { QueryKeys } from '../../../shared/common/queryKeys.ts';

export default function useEditProductLogic(
	product: ProductResponse | null,
	page: number,
	pageSize: number
) {
	// form
	const formSchema = z.object({
		name: z
			.string()
			.nonempty('Name is required')
			.min(3, 'Name must be at least 3 characters.')
			.max(30, "Name can't be larger than 30 characters."),

		description: z
			.string()
			.nonempty('Description is required')
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
			.union([z.file(), z.url()])
			.optional()
			.nullable()
			// server expects undefined value and not null, we need to transform.
			// We also accept this to be null or undefined, since server can load undefined imageUrl and
			// image dropzone when deleting image is loading null value. It's stuped but whatever.
			.transform(val => val ?? undefined),
	});

	type FormValues = z.infer<typeof formSchema>;

	const form = useForm({
		resolver: zodResolver(formSchema),
	});

	// api functions
	const queryClient = useQueryClient();
	const { mutateAsync, isPending, isError, isSuccess } = useProductPatch({
		mutation: {
			onError: handleError,
			// When we change the product, we refetch the page that contained that product
			onSuccess: () => {
				queryClient.invalidateQueries({
					queryKey: QueryKeys.adminGetProductsPaged(page, pageSize),
				});
			},
		},
	});
	const [errorMessage, setErrorMessage] = useState<string | null>(null);
	// used when all the form values are the same as the initial product info
	const [allSet, setAllSet] = useState<boolean>(false);

	const submitEditForm = form.handleSubmit(
		async ({
			name,
			category,
			price,
			image,
			stock,
			description,
		}: FormValues) => {
			setAllSet(false);

			if (!product) {
				throw new Error('Product is null when submitting the form');
			}

			// It's VERY important to use != instead of !== operator
			// image can be undefined and thumbnailUrl can be null
			// if image is undefined and url is null we treat that as the same, so we use != operator since !== will give 'false'
			const imageChanged = image != product.thumbnailUrl;
			const imageNotUrl = typeof image !== 'string';

			// If the initial field names are not changed there is no need to send them
			const request: ProductPatchBody = {
				Name: name !== product.name ? name : undefined,
				Description:
					description !== product.description ? description : undefined,
				Stock: stock !== product.stock ? stock : undefined,
				Category: category !== product.category ? category : undefined,
				Price: price !== product.price ? price : undefined,
				'Image.HasValue': imageChanged,
				'Image.Value': imageChanged && imageNotUrl ? image : undefined,
			};

			// console.log(request);

			const notingIsChanged = isProductPatchRequestEmpty(request);

			if (notingIsChanged) {
				setAllSet(true);
				return;
			}

			await mutateAsync({ id: product.id, data: request });
		}
	);

	function handleError(error: ErrorType<void | ProblemDetails>) {
		if (!error) {
			setErrorMessage(null);
			return;
		}

		if (isAxiosError(error)) {
			switch (error.response?.status) {
				case 404:
					setErrorMessage(
						"This product doesn't exist, please refresh the page and try again."
					);
					return;
				case 400:
					setErrorMessage(
						'You entered wrong input for the product, change form values and try again.'
					);
					return;
			}
		}

		setErrorMessage('Unexpected error occurred, please try again.');
	}

	const loadInitDataToForm = useCallback(() => {
		if (!product) return;

		form.reset();
		form.setValue('name', product.name);
		form.setValue('description', product.description);
		form.setValue('category', product.category);
		form.setValue('price', product.price);
		form.setValue('stock', product.stock);
		form.setValue('image', product.thumbnailUrl ?? undefined);
	}, [product, form.setValue]);

	useEffect(() => {
		if (!product) return;

		setAllSet(false);

		loadInitDataToForm();
	}, [product, setAllSet]);

	return {
		form,
		isSuccess,
		isError,
		allSet,
		setAllSet,
		errorMessage,
		isLoading: isPending,
		submitEditForm,
		loadInitDataToForm,
	};
}

function isProductPatchRequestEmpty(request: ProductPatchBody) {
	return (
		request.Name === undefined &&
		request.Description === undefined &&
		request.Stock === undefined &&
		request.Category === undefined &&
		request.Price === undefined &&
		request['Image.HasValue'] === false &&
		request['Image.Value'] === undefined
	);
}

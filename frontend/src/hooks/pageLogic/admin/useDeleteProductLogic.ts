import { useProductDelete } from '../../../api/openApi/product/product.ts';
import type { ProblemDetails } from '../../../api/openApi/model';
import type { ErrorType } from '../../../api/axiosInstance.ts';
import { useState } from 'react';
import { isAxiosError } from 'axios';
import { useQueryClient } from '@tanstack/react-query';
import { QueryKeys } from '../../../shared/common/queryKeys.ts';

export default function useDeleteProductLogic(page: number, pageSize: number) {
	const [errorMessage, setErrorMessage] = useState<string | null>(null);

	const queryClient = useQueryClient();
	const { isPending, isError, mutateAsync, isSuccess } = useProductDelete({
		mutation: {
			onError: handleError,
			onSuccess: () => {
				queryClient.invalidateQueries({
					queryKey: QueryKeys.adminGetProductsPaged(page, pageSize),
				});
			},
		},
	});

	const deleteProduct = async (productId: string) => {
		await mutateAsync({ id: productId });
	};

	function handleError(error: ErrorType<void | ProblemDetails>) {
		if (!error) {
			setErrorMessage(null);
			return;
		}

		if (isAxiosError(error)) {
			switch (error.response?.status) {
				case 404:
					setErrorMessage(
						"Product doesn't exist anymore, please refresh the page to get the latest data"
					);
					return;
			}
		}

		setErrorMessage('Unexpected error happened');
	}

	return {
		isError,
		isPending,
		isSuccess,
		errorMessage,
		deleteProduct,
	};
}

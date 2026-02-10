import { zodResolver } from '@hookform/resolvers/zod';
import { useForm } from 'react-hook-form';
import z from 'zod';
import { useUserRegister } from '../api/openApi/user/user';
import type { ProblemDetails, RegisterUserRequest } from '../api/openApi/model';
import { useState } from 'react';
import type { ErrorType } from '../api/axiosInstance';
import { isAxiosError } from 'axios';

function useRegisterLogic() {
	const formSchema = z.object({
		name: z
			.string()
			.min(5, 'Name must be at least 5 characters.')
			.max(20, "Name can't exceed 20 characters"),

		surname: z
			.string()
			.min(5, 'Surname must be at least 5 characters.')
			.max(20, "Surname can't exceed 20 characters"),

		email: z.email(),

		password: z.string().min(6, 'Password must be at least 6 characters.'),
	});
	type FormValues = z.Infer<typeof formSchema>;
	const form = useForm({
		resolver: zodResolver(formSchema),
	});

	const { mutateAsync, isError, error, isSuccess, isPending } =
		useUserRegister();

	const handleFormRegister = form.handleSubmit(async (data: FormValues) => {
		const request: RegisterUserRequest = {
			name: data.name,
			surname: data.surname,
			email: data.email,
			password: data.password,
		};
		await mutateAsync({ data: request });
	});

	function extractErrorMessage(
		error: ErrorType<void | ProblemDetails> | null
	): string {
		if (!error) return '';

		if (isAxiosError(error)) {
			// will handle later
		}
		return 'Unkown error occured, please try again.';
	}

	return {
		isError,
		isSuccess,
		isPending,
		errorMesage: extractErrorMessage(error),
		form,
		handleFormRegister,
	};
}

export default useRegisterLogic;

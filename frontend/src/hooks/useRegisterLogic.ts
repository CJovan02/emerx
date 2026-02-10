import { zodResolver } from '@hookform/resolvers/zod';
import { useForm } from 'react-hook-form';
import z from 'zod';
import { useUserRegister } from '../api/openApi/user/user';
import type { ProblemDetails, RegisterUserRequest } from '../api/openApi/model';
import type { ErrorType } from '../api/axiosInstance';
import { isAxiosError } from 'axios';
import { useState } from 'react';

function useRegisterLogic() {
	const lettersRegex = /^[a-zA-ZčČćĆđĐšŠžŽ]+$/;
	const formSchema = z.object({
		name: z
			.string()
			.min(3, 'Name must be at least 5 characters.')
			.max(20, "Name can't exceed 20 characters")
			.regex(lettersRegex, 'Name can only have letters'),

		surname: z
			.string()
			.min(3, 'Surname must be at least 5 characters.')
			.max(20, "Surname can't exceed 20 characters")
			.regex(lettersRegex, 'Surname can only have letters'),

		email: z.email(),

		password: z
			.string()
			.min(6, 'Password must be at least 6 characters.')
			.max(30, "Password can't exceed 30 characters")
			.regex(/[a-z]/, 'Password must contain at least one lowercase letter.')
			.regex(/[A-Z]/, 'Password must contain at least one uppercase letter.'),
	});
	type FormValues = z.Infer<typeof formSchema>;
	const form = useForm({
		resolver: zodResolver(formSchema),
	});
	const [errorMessage, setErrorMessage] = useState<string | null>(null);
	const { mutateAsync, isError, isSuccess, isPending } = useUserRegister({
		mutation: {
			onError: handleError,
		},
	});

	const handleFormRegister = form.handleSubmit(async (data: FormValues) => {
		const request: RegisterUserRequest = {
			name: data.name,
			surname: data.surname,
			email: data.email,
			password: data.password,
		};
		await mutateAsync({ data: request });
	});

	function handleError(error: ErrorType<void | ProblemDetails> | null) {
		if (!error) {
			setErrorMessage(null);
			return;
		}

		if (isAxiosError(error)) {
			const data = error.response?.data;

			// 409 -> email occupied
			if (error.response?.status === 409) {
				form.setError('email', { message: 'That Email is already in use.' });
				return;
			}

			// Fluent validation has "errors" property. I don't know how to map that with openApi spec
			if (error.response?.status === 400 && data?.errors) {
				for (const [field, messages] of Object.entries(data.errors)) {
					form.setError(field.toLowerCase() as any, {
						message: messages[0],
					});
				}
				return;
			}
		}

		setErrorMessage('Unkown error occured, please try again.');
	}

	return {
		isError,
		isSuccess,
		isLoading: isPending,
		errorMessage,
		form,
		handleFormRegister,
	};
}

export default useRegisterLogic;

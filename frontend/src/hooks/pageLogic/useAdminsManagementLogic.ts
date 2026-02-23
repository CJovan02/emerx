import {useUserGrantAdminRole, useUserRemoveAdminRole} from "../../api/openApi/user/user.ts";
import {useForm} from "react-hook-form";
import z from "zod";
import {zodResolver} from "@hookform/resolvers/zod";
import type {ErrorType} from "../../api/axiosInstance.ts";
import type {ProblemDetails} from "../../api/openApi/model";
import {isAxiosError} from "axios";
import {useCallback, useState} from "react";
import {useUserStore} from "../../stores/userStore.ts";

export default function useAdminsManagementLogic() {
    // Form
    const formSchema = z.object({
        email: z.email().max(50, "Email can't be longer than 50 characters")
    })
    type FormValues = z.infer<typeof formSchema>;
    const form = useForm({
        resolver: zodResolver(formSchema),
    });

    // Server functions
    const {
        mutateAsync: grantMutate,
        isPending: grantIsPending,
        isSuccess: grantIsSuccess
    } = useUserGrantAdminRole({
        mutation: {
            onError: handleError
        }
    });
    const {
        mutateAsync: removeMutate,
        isPending: removeIsPending,
        isSuccess: removeIsSuccess
    } = useUserRemoveAdminRole({
        mutation: {
            onError: handleError
        }
    });
    const adminEmail = useUserStore(store => store.user!.email);

    // Custom states
    const [formAction, setFormAction] = useState<'grant' | 'remove' | null>(null);
    const [errorMessage, setErrorMessage] = useState<string | null>(null);
    const isLoading = grantIsPending || removeIsPending;


    // Business logic
    const handleSubmit = form.handleSubmit(async ({email}: FormValues) => {
        if (!formAction) return;
        if (adminEmail === email) {
            form.setError('email', {message: "You can't edit your own roles."});
            return;
        }

        if (formAction === 'grant') {
            await grantAdminRole(email);
        } else if (formAction === 'remove') {
            await removeAdminRole(email);
        }
    });

    const grantAdminRole = useCallback(async (email: string) => {
        if (isLoading) return;
        setErrorMessage(null);

        await grantMutate({email});
    }, [isLoading, grantMutate]);

    const removeAdminRole = useCallback(async (email: string) => {
        if (isLoading) return;
        setErrorMessage(null);

        await removeMutate({email});
    }, [isLoading, removeMutate]);

    function handleError(error: ErrorType<void | ProblemDetails>) {
        if (!error) {
            setErrorMessage(null);
            return;
        }

        if (isAxiosError(error)) {
            switch (error.response?.status) {
                case 404:
                    form.setError('email', {message: "User with provided email doesn't exist."});
                    return;
                case 403:
                    setErrorMessage("Će probaš malo mac ako nisi admin.")
                    return;
                case 409:
                    setErrorMessage("You can't edit your own roles.");
                    return;
            }
        }

        setErrorMessage('Unknown error occurred, please try again.');

    }


    return {
        form,
        setFormAction,
        hasErrorMessage: errorMessage !== null,
        errorMessage,
        grantIsLoading: grantIsPending,
        grantIsSuccess,
        removeIsLoading: removeIsPending,
        removeIsSuccess,
        handleSubmit,
    }
}
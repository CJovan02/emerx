import {useState} from "react";
import type {PatchProductRequest, ProblemDetails, ProductResponse} from "../../api/openApi/model";
import {useForm} from "react-hook-form";
import z from "zod";
import {zodResolver} from "@hookform/resolvers/zod";
import {useProductPatch} from "../../api/openApi/product/product.ts";
import type {ErrorType} from "../../api/axiosInstance.ts";
import {isAxiosError} from "axios";

export default function useAdminProductDrawerLogic(product: ProductResponse | null) {
    // form
    const formSchema = z.object({
        name: z
            .string()
            .min(3, "Name must be at least 3 characters.")
            .max(30, "Name can't be larger than 30 characters."),

        category: z
            .string()
            .min(3, "Category must be at least 3 characters.")
            .max(30, "Category can't be larger than 30 characters."),

        price: z.number().min(0, "Price must be larger than 0."),
    })

    type FormValues = z.infer<typeof formSchema>;

    const form = useForm({
        resolver: zodResolver(formSchema),
        defaultValues: {
            name: product?.name ?? '',
            category: product?.category ?? '',
            price: product?.price ?? 0,
        }
    });


    // api functions
    const {mutateAsync, isPending, isError, isSuccess} = useProductPatch({
        mutation: {
            onError: handleError,
        }
    });
    const [errorMessage, setErrorMessage] = useState<string | null>(null);

    const submitEditForm = form.handleSubmit(async ({name, category, price}: FormValues) => {
        if (!product) {
            throw new Error("Product is null when submitting the form");
        }

        // If the initial field names are not changed there is no need to send them
        const request: PatchProductRequest = {
            name: name !== product.name ? name : null,
            category: category !== product.category ? category : null,
            price: price !== product.price ? price : null,
        }

        await mutateAsync({id: product.id, data: request});
    })

    function handleError(error: ErrorType<void | ProblemDetails>) {
        if (!error) {
            setErrorMessage(null);
            return;
        }

        if (isAxiosError(error)) {
            switch (error.response?.status) {
                case 404:
                    setErrorMessage("This product doesn't exist, please refresh the page and try again.");
                    return;
                case 400:
                    setErrorMessage("You entered wrong input for the product, change form values and try again.");
                    return;
            }
        }

        setErrorMessage("Unexpected error occurred, please try again.");
    }

    return {
        form,
        product,
        open,
        isSuccess,
        isError,
        errorMessage,
        isLoading: isPending,
        submitEditForm,
    }
}

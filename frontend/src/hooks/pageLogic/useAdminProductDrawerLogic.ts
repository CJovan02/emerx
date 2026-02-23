import {useEffect, useState} from "react";
import type {
    PatchProductRequest,
    ProblemDetails,
    ProductResponse
} from "../../api/openApi/model";
import {useForm} from "react-hook-form";
import z from "zod";
import {zodResolver} from "@hookform/resolvers/zod";
import {useProductPatch} from "../../api/openApi/product/product.ts";
import type {ErrorType} from "../../api/axiosInstance.ts";
import {isAxiosError} from "axios";
import {useQueryClient} from "@tanstack/react-query";
import {QueryKeys} from "../../shared/common/queryKeys.ts";

export default function useAdminProductDrawerLogic(product: ProductResponse | null, page: number, pageSize: number) {
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
    });


    // api functions
    const queryClient = useQueryClient();
    const {mutateAsync, isPending, isError, isSuccess} = useProductPatch({
        mutation: {
            onError: handleError,
            // When we change the product, we refetch the page that contained that product
            onSuccess: async () => {
                await queryClient.invalidateQueries({queryKey: QueryKeys.adminGetProductsPaged(page, pageSize)})
            }
        }
    });
    const [errorMessage, setErrorMessage] = useState<string | null>(null);
    // used when all the form values are the same as the initial product info
    const [allSet, setAllSet] = useState<boolean>(false);

    const submitEditForm = form.handleSubmit(async ({name, category, price}: FormValues) => {
        setAllSet(false);

        if (!product) {
            throw new Error("Product is null when submitting the form");
        }

        // If the initial field names are not changed there is no need to send them
        const request: PatchProductRequest = {
            name: name !== product.name ? name : null,
            category: category !== product.category ? category : null,
            price: price !== product.price ? price : null,
        }

        const isEmptyRequest = Object.values(request).every(val => val === null)
        if (isEmptyRequest) {
            setAllSet(true);
            return;
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

    useEffect(() => {
        if (!product) return;

        setAllSet(false);

        form.setValue('name', product.name);
        form.setValue('category', product.category);
        form.setValue('price', product.price);
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
    }
}

import {useProductGetPaged} from "../../api/openApi/product/product.ts";
import {useCallback, useEffect, useState} from "react";
import type {ProductResponse} from "../../api/openApi/model";
import {QueryKeys} from "../../shared/common/queryKeys.ts";

export default function useAdminProductsLogic() {
    const [page, setPage] = useState<number>(0); // we use 0 since Data Grid uses 0-based index
    const [pageSize, setPageSize] = useState<number>(10);
    const [errorMessage, setErrorMessage] = useState<string | null>(null);
    const [product, setProduct] = useState<ProductResponse | null>(null);

    const query = useProductGetPaged(
        {
            Page: page + 1, // backend uses 1-based index
            PageSize: pageSize,
        },
        {
            query: {
                queryKey: QueryKeys.adminGetProductsPaged(page, pageSize)
            }
        }
    )
    const {isError, error} = query

    // drawer UI logic
    const [open, setOpen] = useState<boolean>(false);

    const openDrawer = useCallback((product: ProductResponse) => {
        setProduct(product);

        setOpen(true);
    }, [setProduct, setOpen]);

    const closeDrawer = useCallback(() => {
        setOpen(false);
    }, [setOpen]);

    useEffect(() => {
        if (!isError) {
            setErrorMessage(null);
            return;
        }

        console.error(error);
        setErrorMessage("Unexpected error happened, please try again.");
    }, [error, errorMessage, setErrorMessage, isError]);

    return {
        ...query,
        errorMessage,
        page,
        pageSize,
        product,
        open,
        openDrawer,
        closeDrawer,
        setProduct,
        setPage,
        setPageSize,
    }
}
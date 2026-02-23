import {useProductGetPaged} from "../../api/openApi/product/product.ts";
import {useEffect, useState} from "react";

export default function useAdminProductsLogic() {
    const [page, setPage] = useState<number>(0); // we use 0 since Data Grid uses 0-based index
    const [pageSize, setPageSize] = useState<number>(10);
    const [errorMessage, setErrorMessage] = useState<string | null>(null);

    const query = useProductGetPaged(
        {
            Page: page + 1, // backend uses 1-based index
            PageSize: pageSize,
        },
    )
    const {isError, error} = query

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
        setPage,
        setPageSize,
    }
}
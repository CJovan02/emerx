import {useProductGetPaged} from "../../../api/openApi/product/product.ts";
import {useCallback, useEffect, useState} from "react";
import type {ProductResponse} from "../../../api/openApi/model";
import {QueryKeys} from "../../../shared/common/queryKeys.ts";

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
    const [editOpen, setEditOpen] = useState<boolean>(false);
    const [addOpen, setAddOpen] = useState<boolean>(false);
    const [deleteOpen, setDeleteOpen] = useState<boolean>(false);

    const openEditDrawer = useCallback((product: ProductResponse) => {
        setProduct(product);

        setEditOpen(true);
    }, [setProduct, setEditOpen]);
    const openAddDrawer = useCallback(() => {
        setAddOpen(true);
    }, [setAddOpen])
    const openDeleteDialog = useCallback((product: ProductResponse) => {
        setProduct(product);
        setDeleteOpen(true)
    }, [setDeleteOpen, setProduct])

    const closeEditDrawer = useCallback(() => {
        setEditOpen(false);
    }, [setEditOpen]);
    const closeAddDrawer = useCallback(() => {
        setAddOpen(false);
    }, [setAddOpen]);
    const closeDeleteDialog = useCallback(() => {
        setDeleteOpen(false);
    }, [setDeleteOpen]);

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
        editOpen,
        addOpen,
        deleteOpen,
        openDeleteDialog,
        closeDeleteDialog,
        openAddDrawer,
        closeAddDrawer,
        openEditDrawer,
        closeEditDrawer,
        setProduct,
        setPage,
        setPageSize,
    }
}
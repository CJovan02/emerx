import {useProductGetById} from "../../api/openApi/product/product.ts";
import { useEffect, useState } from 'react';
import {isAxiosError} from "axios";

function useProductDetailsLogic(productId: string) {
    const [errorMessage, setErrorMessage] = useState<string | null>(null);
    const [productNotFound, setProductNotFound] = useState(false);
    
    // react query
    const query = useProductGetById(productId);
    const {isError, error} = query;

    // business logic
    useEffect(() => {
        if (!isError) {
            setErrorMessage(null);
            return;
        }

        console.error(error);

        if (isAxiosError(error)) {
            switch(error.response?.status) {
                case 404:
                    setErrorMessage("Product not found");
                    setProductNotFound(true);
                    return;
            }
        }

        setErrorMessage('Unexpected error happened, please try again.');
    }, [isError, error, setErrorMessage, setProductNotFound]);
    
    return {
        ...query,
        errorMessage,
        productNotFound,
    }
}

export default useProductDetailsLogic;
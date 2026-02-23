import {useState} from "react";
import type {ProductResponse} from "../../api/openApi/model";
import {useForm} from "react-hook-form";

export default function useAdminProductDrawerLogic() {
    // drawer UI logic
    const [open, setOpen] = useState<boolean>(false);

    const openDrawer = (product: ProductResponse) => {
        setProduct(product);
        setOpen(true);
    };

    const closeDrawer = () => {
        setOpen(false);
    };


    // custom states
    // external components that open this drawer need to provide the product that is being displayed in the drawer
    const [product, setProduct] = useState<ProductResponse | null>(null);

    return {
        product,
        open,
        openDrawer,
        closeDrawer,
    }
}

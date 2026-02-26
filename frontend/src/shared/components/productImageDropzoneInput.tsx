import {Controller, useFormContext} from "react-hook-form";
import ProductImageDropzone from "./ui/productImageDropzone.tsx";
import {Typography} from "@mui/material";

type Props = {
    id: string;
    required?: boolean;
    disabled?: boolean;
    fullWidth?: boolean;
}

export function ProductImageDropzoneInput({id}: Props) {
    const {control} = useFormContext();

    return (
        <Controller
            name={id}
            control={control}
            render={({field, fieldState}) => (
                <>
                    <ProductImageDropzone
                        id={id}
                        value={field.value}
                        onChange={field.onChange}
                    />
                    {fieldState.error && <Typography sx={{color: 'error.main'}}>{fieldState.error.message}</Typography>}
                </>
            )}
        >

        </Controller>
    );
};
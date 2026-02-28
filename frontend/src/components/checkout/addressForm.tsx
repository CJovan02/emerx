import { Box, Button, TextField, Typography } from "@mui/material";
import { useForm } from "react-hook-form";
import type {AddressRequiredDto} from "../../api/openApi/model";

type Props = {
    defaultValues?: AddressRequiredDto;
    onContinue: (values: AddressRequiredDto) => void;
};

export default function AddressForm({
                                        defaultValues,
                                        onContinue,
                                    }: Props) {
    const {
        register,
        handleSubmit,
        formState: { errors },
    } = useForm<AddressRequiredDto>({
        defaultValues,
    });

    return (
        <Box component="form" onSubmit={handleSubmit(onContinue)}>
            <Typography variant="h6" mb={2}>
                Shipping Address
            </Typography>


            <TextField
                fullWidth
                label="City"
                margin="normal"
                {...register("city", { required: "Required" })}
                error={!!errors.city}
                helperText={errors.city?.message}
            />

            <TextField
                fullWidth
                label="Street"
                margin="normal"
                {...register("street", { required: "Required" })}
                error={!!errors.street}
                helperText={errors.street?.message}
            />

            <TextField
                fullWidth
                label="Country"
                margin="normal"
                {...register("houseNumber", { required: "Required" })}
                error={!!errors.houseNumber}
                helperText={errors.houseNumber?.message}
            />

            <Button
                fullWidth
                variant="contained"
                type="submit"
                sx={{ mt: 3 }}
            >
                Continue to Review
            </Button>
        </Box>
    );
}
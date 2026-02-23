import useAdminsManagementLogic from "../../hooks/pageLogic/admin/useAdminsManagementLogic.ts";
import {useEffect} from "react";
import {useSnackbar} from "notistack";
import {Box, Button} from "@mui/material";
import {FormProvider} from "react-hook-form";
import TextInput from "../../shared/components/ui/textInput.tsx";

export default function AdminsManagementPage() {
    const {
        form,
        errorMessage,
        hasErrorMessage,
        removeIsSuccess,
        grantIsSuccess,
        removeIsLoading,
        grantIsLoading,
        setFormAction,
        handleSubmit
    } = useAdminsManagementLogic();
    const {enqueueSnackbar} = useSnackbar();

    useEffect(() => {
        if (!hasErrorMessage) return;

        enqueueSnackbar(errorMessage, {variant: 'error'})
    }, [hasErrorMessage, errorMessage]);

    useEffect(() => {
        if (!grantIsSuccess) return;

        enqueueSnackbar("Successfully granted admin role.", {variant: 'success'})
    }, [grantIsSuccess])

    useEffect(() => {
        if (!removeIsSuccess) return;

        enqueueSnackbar("Successfully removed admin role.", {variant: 'success'})
    }, [removeIsSuccess])

    return (
        <Box display='flex' flexDirection='column' maxWidth='35rem' mx='auto'>
            <FormProvider {...form}>
                <form id='admins-management-form' onSubmit={handleSubmit}>
                    <TextInput
                        id='email'
                        label='Email'
                        fullWidth
                        required
                    />

                    <Box display='flex' gap={3} mt={3}>
                        <Button
                            type='submit'
                            onClick={() => setFormAction('grant')}
                            sx={{
                                flexGrow: 1
                            }}
                            loading={grantIsLoading}
                            disabled={removeIsLoading}
                        >
                            Grant Admin Role
                        </Button>

                        <Button
                            type='submit'
                            color='error'
                            onClick={() => setFormAction('remove')}
                            sx={{
                                flexGrow: 1
                            }}
                            loading={removeIsLoading}
                            disabled={grantIsLoading}
                        >
                            Remove Admin Role
                        </Button>
                    </Box>
                </form>
            </FormProvider>
        </Box>
    )
}
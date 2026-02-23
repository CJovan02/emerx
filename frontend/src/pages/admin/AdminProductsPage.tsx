import {Alert, Box, Button, Container, Stack} from "@mui/material";
import ProductsGrid from "../../components/admin/productsGrid.tsx";
import useAdminProductsLogic from "../../hooks/pageLogic/useAdminProductsLogic.ts";
import {Refresh} from "@mui/icons-material";
import CircularProgress from "@mui/material/CircularProgress";
import AdminProductDrawer from "../../components/admin/adminProductDrawer.tsx";
import useAdminProductDrawerLogic from "../../hooks/pageLogic/useAdminProductDrawerLogic.ts";

export default function AdminProductsPage() {
    const {data, isSuccess, isError, errorMessage, isLoading, isPending, refetch} = useAdminProductsLogic();
    const {closeDrawer, openDrawer, open, product} = useAdminProductDrawerLogic();

    return (
        <>
            <AdminProductDrawer
                open={open}
                handleClose={closeDrawer}
                product={product}
            />

            <Container>
                {isPending && (
                    <Box display='flex' justifyContent='center'>
                        <CircularProgress size={50}/>
                    </Box>
                )}

                {isError && (
                    <Stack spacing={4} maxWidth='25rem' mx='auto'>
                        <Alert severity='error'>{errorMessage}</Alert>
                        <Button
                            startIcon={<Refresh/>}
                            onClick={() => refetch()}
                            sx={{
                                height: 45,
                            }}
                        >
                            Refresh
                        </Button>
                    </Stack>
                )}

                {isSuccess &&
                    <ProductsGrid
                        openDrawer={openDrawer}
                        data={data!.items}
                        totalItems={1000}
                        loading={isLoading}
                        fetchPage={() => {
                        }}
                    />
                }


            </Container>
        </>
    )
}
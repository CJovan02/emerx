import {Alert, Box, Button, Container, Stack} from "@mui/material";
import ProductsGrid from "../../components/admin/productsGrid.tsx";
import useAdminProductsLogic from "../../hooks/pageLogic/useAdminProductsLogic.ts";
import {Refresh} from "@mui/icons-material";
import CircularProgress from "@mui/material/CircularProgress";
import AdminProductDrawer from "../../components/admin/adminProductDrawer.tsx";
import {useCallback} from "react";

export default function AdminProductsPage() {
    const {
        data,
        page,
        pageSize,
        isSuccess,
        isError,
        errorMessage,
        isLoading,
        isPending,
        refetch,
        open,
        openDrawer,
        closeDrawer,
        product
    } = useAdminProductsLogic();

    const fetchPage = useCallback(() => {
    }, []);

    return (
        <>
            <AdminProductDrawer
                open={open}
                handleClose={closeDrawer}
                product={product}
                page={page}
                pageSize={pageSize}
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
                        totalItems={data!.totalItems}
                        loading={isLoading}
                        fetchPage={fetchPage}
                    />
                }


            </Container>
        </>
    )
}
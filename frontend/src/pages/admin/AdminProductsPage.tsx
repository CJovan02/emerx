import {Alert, Box, Button, Container, Stack} from "@mui/material";
import ProductsGrid from "../../components/admin/productsGrid.tsx";
import useAdminProductsLogic from "../../hooks/pageLogic/admin/useAdminProductsLogic.ts";
import {Add, Refresh} from "@mui/icons-material";
import CircularProgress from "@mui/material/CircularProgress";
import EditProductDrawer from "../../components/admin/editProductDrawer.tsx";
import {useCallback} from "react";
import {Spacer} from "../../shared/components/ui/spacer.tsx";
import CreateProductDrawer from "../../components/admin/createProductDrawer.tsx";

export default function AdminProductsPage() {
    const {
        data,
        page,
        pageSize,
        isSuccess,
        isError,
        errorMessage,
        isPending,
        isFetching,
        refetch,
        addOpen,
        editOpen,
        openEditDrawer,
        closeEditDrawer,
        openAddDrawer,
        closeAddDrawer,
        product
    } = useAdminProductsLogic();

    const fetchPage = useCallback(() => {
    }, []);

    return (
        <>
            <CreateProductDrawer
                open={addOpen}
                handleClose={closeAddDrawer}
                page={data ? data.totalPages - 1 : 0} // -1 because we transform from 1-based to 0-based index
                pageSize={pageSize}
            />

            <EditProductDrawer
                open={editOpen}
                handleClose={closeEditDrawer}
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
                    <>
                        <Box display='flex' mb={2}>
                            <Spacer/>
                            <Button
                                startIcon={<Add/>}
                                onClick={openAddDrawer}
                                sx={{
                                    height: 45,
                                    fontWeight: 700,
                                }}
                            >
                                Create Product
                            </Button>
                        </Box>
                        <ProductsGrid
                            openDrawer={openEditDrawer}
                            data={data!.items}
                            totalItems={data!.totalItems}
                            loading={isFetching}
                            fetchPage={fetchPage}
                        />
                    </>
                }
            </Container>
        </>
    )
}
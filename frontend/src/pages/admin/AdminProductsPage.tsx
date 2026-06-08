import {Alert, Box, Button, Container, Stack, TextField} from '@mui/material';
import ProductsGrid from '../../components/admin/productsGrid.tsx';
import useAdminProductsLogic from '../../hooks/pageLogic/admin/useAdminProductsLogic.ts';
import {Add, Refresh} from '@mui/icons-material';
import EditProductDrawer from '../../components/admin/editProductDrawer.tsx';
import { Spacer } from '../../shared/components/ui/spacer.tsx';
import CreateProductDrawer from '../../components/admin/createProductDrawer.tsx';
import DeleteProductDialog from '../../components/admin/deleteProductDialog.tsx';
import {useEffect, useState} from "react";

const SEARCH_DEBOUNCE_MS = 350;

export default function AdminProductsPage() {
	const [searchInputValue, setSearchInputValue] = useState("Wire");
	const {
		data,
		page,
		pageSize,
		setPage,
		setPageSize,
		isError,
		errorMessage,
		isFetching,
		refetch,
		addOpen,
		editOpen,
		deleteOpen,
		openDeleteDialog,
		closeDeleteDialog,
		openEditDrawer,
		closeEditDrawer,
		openAddDrawer,
		closeAddDrawer,
		product,
		setSearch,
	} = useAdminProductsLogic();

	useEffect(() => {
		const timer = setTimeout(() => {
			setSearch(searchInputValue);
		}, SEARCH_DEBOUNCE_MS);

		return () => clearTimeout(timer);
	}, [searchInputValue, setSearch])

    if (isError) {
        return (
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
        )
    }

    return (
        <>
            <CreateProductDrawer
                open={addOpen}
                handleClose={closeAddDrawer}
            />

			<EditProductDrawer
				open={editOpen}
				handleClose={closeEditDrawer}
				product={product}
			/>

			<DeleteProductDialog
				open={deleteOpen}
				onClose={closeDeleteDialog}
				product={product}
			/>

            <Container>
                <Box display='flex' mb={2}>
					<TextField
						label="Search products"
						value={searchInputValue}
						onChange={e => setSearchInputValue(e.target.value)}
						size="small"
					/>
                    <Spacer/>
                    <Button
						data-testid='open-create-drawer-button'
                        startIcon={<Add/>}
                        onClick={openAddDrawer}
                        disabled={isFetching}
                        sx={{
                            height: 45,
                            fontWeight: 700,
                        }}
                    >
                        Create Product
                    </Button>
                </Box>
                <ProductsGrid
                    openDeleteDialog={openDeleteDialog}
                    openDrawer={openEditDrawer}
                    data={data?.items}
                    totalItems={data?.totalItems}
                    loading={isFetching}
                    page={page}
                    pageSize={pageSize}
                    setPage={setPage}
                    setPageSize={setPageSize}
                />
            </Container>
        </>
    )
}

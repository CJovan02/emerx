import { Alert, Box, Button, Container, Stack } from '@mui/material';
import ProductsGrid from '../../components/admin/productsGrid.tsx';
import useAdminProductsLogic from '../../hooks/pageLogic/admin/useAdminProductsLogic.ts';
import { Add, Refresh } from '@mui/icons-material';
import EditProductDrawer from '../../components/admin/editProductDrawer.tsx';
import { Spacer } from '../../shared/components/ui/spacer.tsx';
import CreateProductDrawer from '../../components/admin/createProductDrawer.tsx';
import DeleteProductDialog from '../../components/admin/deleteProductDialog.tsx';

export default function AdminProductsPage() {
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
	} = useAdminProductsLogic();

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

			<DeleteProductDialog
				open={deleteOpen}
				onClose={closeDeleteDialog}
				product={product}
				page={page}
				pageSize={pageSize}
			/>

			<Container>
				{isError && (
					<Stack
						spacing={4}
						maxWidth='25rem'
						mx='auto'>
						<Alert severity='error'>{errorMessage}</Alert>
						<Button
							startIcon={<Refresh />}
							onClick={() => refetch()}
							sx={{
								height: 45,
							}}>
							Refresh
						</Button>
					</Stack>
				)}

				<Box
					display='flex'
					mb={2}>
					<Spacer />
					<Button
						startIcon={<Add />}
						onClick={openAddDrawer}
						disabled={isFetching}
						sx={{
							height: 45,
							fontWeight: 700,
						}}>
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
	);
}

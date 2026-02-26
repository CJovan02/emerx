import type { ProductResponse } from '../../api/openApi/model';
import {
	Alert,
	Button,
	Dialog,
	DialogActions,
	DialogContent,
	DialogTitle,
} from '@mui/material';
import useDeleteProductLogic from '../../hooks/pageLogic/admin/useDeleteProductLogic.ts';
import { useEffect } from 'react';
import { useSnackbar } from 'notistack';

type Props = {
	open: boolean;
	onClose: () => void;
	product: ProductResponse | null;
	page: number;
	pageSize: number;
};

export default function DeleteProductDialog({
	product,
	onClose,
	open,
	page,
	pageSize,
}: Props) {
	const { deleteProduct, isError, errorMessage, isPending, isSuccess } =
		useDeleteProductLogic(page, pageSize);
	const { enqueueSnackbar } = useSnackbar();

	async function onDelete() {
		if (!product) {
			throw Error('Product is null when trying to delete it');
		}
		await deleteProduct(product.id);
	}

	useEffect(() => {
		if (!isError) return;

		enqueueSnackbar(errorMessage, { variant: 'error' });
		onClose();
	}, [isError, errorMessage, onClose]);

	useEffect(() => {
		if (!isSuccess) return;

		enqueueSnackbar(`Successfully deleted product "${product?.name}"`, {
			variant: 'success',
		});
		onClose();
	}, [isSuccess, onClose]);

	return (
		<Dialog
			open={open}
			onClose={onClose}>
			<DialogTitle>
				Are you sure you want to delete "{product?.name}"?
			</DialogTitle>
			<DialogContent>
				<Alert severity='warning'>
					Deleting this product will also delete all of the reviews related to
					it.
				</Alert>
				<Alert severity='info'>Review deletion is not implemented yet.</Alert>
			</DialogContent>
			<DialogActions sx={{ px: 3, py: 2 }}>
				<Button
					variant='outlined'
					onClick={onClose}>
					Cancel
				</Button>
				<Button
					color='error'
					onClick={onDelete}
					loading={isPending}>
					Delete
				</Button>
			</DialogActions>
		</Dialog>
	);
}

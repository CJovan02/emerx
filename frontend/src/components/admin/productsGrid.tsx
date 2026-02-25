import {
	DataGrid,
	type GridColDef,
	type GridRenderCellParams,
} from '@mui/x-data-grid';
import { Box, IconButton, Typography } from '@mui/material';
import Inventory2OutlinedIcon from '@mui/icons-material/Inventory2Outlined';
import EditIcon from '@mui/icons-material/Edit';
import DeleteIcon from '@mui/icons-material/Delete';
import { useMemo } from 'react';
import type { ProductResponse } from '../../api/openApi/model';
import RatingMemo from '../../shared/components/ui/ratingMemo.tsx';
import * as React from 'react';

interface Props {
	data?: ProductResponse[];
	totalItems?: number;
	loading: boolean;
	openDrawer: (product: ProductResponse) => void;
	page: number;
	pageSize: number;
	setPage: (page: number) => void;
	setPageSize: (pageSize: number) => void;
	openDeleteDialog: (product: ProductResponse) => void;
}

function ProductsGrid({
	data,
	totalItems,
	loading,
	openDrawer,
	page,
	pageSize,
	setPage,
	setPageSize,
	openDeleteDialog,
}: Props) {
	const columns = useMemo<GridColDef<ProductResponse>[]>(
		() => [
			{
				field: 'image',
				headerName: '',
				width: 70,
				filterable: false,
				sortable: false,
				align: 'center',
				renderCell: () => <Inventory2OutlinedIcon />,
			},
			{
				field: 'name',
				headerName: 'Name',
				flex: 1,
			},
			{
				field: 'category',
				headerName: 'Category',
				flex: 1,
			},
			{
				//$ €
				field: 'price',
				headerName: 'Price',
				flex: 1,
				valueFormatter: params => `€${params}`,
			},
			{
				field: 'averageRating',
				headerName: 'Rating',
				flex: 1,
				renderCell: (params: GridRenderCellParams<ProductResponse, number>) => (
					<Box
						display='flex'
						gap={1}>
						<RatingMemo value={params.value} />
						<Typography
							variant='body2'
							color='textSecondary'>
							({params.value})
						</Typography>
					</Box>
				),
			},
			{
				field: 'actions',
				headerName: 'Actions',
				width: 120,
				sortable: false,
				renderCell: params => (
					<>
						<IconButton
							onClick={e => {
								e.stopPropagation();
								openDrawer(params.row);
							}}
							// disableRipple // disable ripple for data grid performance
						>
							<EditIcon />
						</IconButton>

						<IconButton
							onClick={e => {
								e.stopPropagation();
								openDeleteDialog(params.row);
							}}
							// disableRipple // disable ripple for data grid performance
						>
							<DeleteIcon />
						</IconButton>
					</>
				),
			},
		],
		[]
	);

	return (
		<Box sx={{ width: '100%' }}>
			<DataGrid
				autoHeight={false}
				rows={data}
				columns={columns}
				getRowId={row => row.id}
				rowCount={totalItems}
				loading={loading}
				paginationMode='server'
				paginationModel={{ page, pageSize }}
				onPaginationModelChange={model => {
					setPage(model.page);
					setPageSize(model.pageSize);
				}}
				pageSizeOptions={[10, 20, 50]}
				sx={{
					height: '650px',
				}}
				disableRowSelectionOnClick
			/>
		</Box>
	);
}

export default React.memo(ProductsGrid);

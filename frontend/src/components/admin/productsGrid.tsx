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
import ImageLightbox from '../../shared/components/ui/imageLightbox.tsx';
import {formatCurrency} from "../../utils/utils.ts";

// const ProductImage = styled('img')({
//     width: 60,
//     height: 60,
//     objectFit: 'cover',
//     borderRadius: 12,
// });

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
				field: 'thumbnailUrl',
				headerName: '',
				maxWidth: 90,
				filterable: false,
				sortable: false,
				align: 'center',
				renderCell: (params: GridRenderCellParams<ProductResponse, string>) => {
					if (!params.value)
						return <Inventory2OutlinedIcon sx={{ fontSize: 28 }} />;

					return (
						<ImageLightbox
							src={params.value}
							alt={params.value}
						/>
					);
				},
			},
			{
				field: 'name',
				headerName: 'Name',
				flex: 1,
				minWidth: 230,
				renderCell: (params: GridRenderCellParams<ProductResponse, string>) => {
					const description = params.value;
					if (!description) return '-';

					const length = 50;
					const truncated =
						description.length > length
							? description.substring(0, length) + '...'
							: description;

					return <span title={description}>{truncated}</span>;
				},
			},
			{
				field: 'description',
				headerName: 'Description',
				minWidth: 170,
				flex: 1,
				renderCell: (params: GridRenderCellParams<ProductResponse, string>) => {
					const description = params.value;
					if (!description) return '-';

					const length = 20;
					const truncated =
						description.length > length
							? description.substring(0, length) + '...'
							: description;

					return <span title={description}>{truncated}</span>;
				},
			},
			{
				field: 'category',
				headerName: 'Category',
				flex: 1,
			},
			{
				field: 'stock',
				headerName: 'Current Stock',
				flex: 1,
				valueFormatter: params => `${params} units`,
			},
			{
				//$ €
				// fr-FR for spaces
				// de-DE for commas
				field: 'price',
				headerName: 'Price',
				flex: 1,
				valueFormatter: params => {
					return formatCurrency(params);
				},
			},
			{
				field: 'averageRating',
				headerName: 'Rating',
				flex: 1,
				minWidth: 185,
				renderCell: (params: GridRenderCellParams<ProductResponse, number>) => (
					<Box
						display='flex'
						gap={1}>
						<RatingMemo value={params.value} />
						<Typography
							variant='body2'
							color='textSecondary'>
							({params.value}) of {params.row.reviewCount}
						</Typography>
					</Box>
				),
			},
			{
				field: 'actions',
				headerName: 'Actions',
				flex: 1,
				maxWidth: 110,
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
				rowHeight={80}
				rows={data}
				columns={columns}
				getRowId={row => row.id}
				paginationMode='server'
				rowCount={totalItems}
				onRowCountChange={setPageSize}
				loading={loading}
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

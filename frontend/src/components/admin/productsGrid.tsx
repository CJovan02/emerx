import {DataGrid, type GridColDef, type GridRenderCellParams} from "@mui/x-data-grid";
import {Box, IconButton} from "@mui/material";
import Inventory2OutlinedIcon from "@mui/icons-material/Inventory2Outlined";
import EditIcon from "@mui/icons-material/Edit";
import DeleteIcon from "@mui/icons-material/Delete";
import {useMemo, useState} from "react";
import type {ProductResponse} from "../../api/openApi/model";
import RatingMemo from "../../shared/components/ui/ratingMemo.tsx";
import * as React from "react";

interface Props {
    data: ProductResponse[];
    totalItems: number;
    loading: boolean;
    fetchPage: (page: number, pageSize: number) => void;
    openDrawer: (product: ProductResponse) => void;
}

function ProductsGrid({
                                         data,
                                         totalItems,
                                         loading,
                                         fetchPage,
                                         openDrawer,
                                     }: Props) {
    const [paginationModel, setPaginationModel] = useState({
        page: 0,
        pageSize: 10
    });

    const columns = useMemo<GridColDef<ProductResponse>[]>(() => [
        {
            field: "image",
            headerName: "",
            width: 70,
            filterable: false,
            sortable: false,
            align: "center",
            renderCell: () => <Inventory2OutlinedIcon/>
        },
        {
            field: "name",
            headerName: "Name",
            flex: 1
        },
        {
            field: "category",
            headerName: "Category",
            flex: 1
        },
        {
            //$ €
            field: "price",
            headerName: "Price",
            flex: 1,
            valueFormatter: (params) => `€${params}`
        },
        {
            field: 'averageRating',
            headerName: "Rating",
            flex: 1,
            renderCell: (params: GridRenderCellParams<ProductResponse, number>) =>
                <RatingMemo value={params.value}/>
        },
        {
            field: "actions",
            headerName: "Actions",
            width: 120,
            sortable: false,
            renderCell: (params) => (
                <>
                    <IconButton
                        onClick={(e) => {
                            e.stopPropagation();
                            openDrawer(params.row);
                        }}
                        disableRipple // disable ripple for data grid performance
                    >
                        <EditIcon/>
                    </IconButton>

                    <IconButton
                        onClick={(e) => {
                            e.stopPropagation();
                            console.log(params.row.id);
                        }}
                        disableRipple // disable ripple for data grid performance
                    >
                        <DeleteIcon/>
                    </IconButton>
                </>
            )
        }
    ], []);

    return (
        <Box sx={{width: "100%"}}>
            <DataGrid
                autoHeight={false}
                rows={data}
                columns={columns}
                getRowId={(row) => row.id}
                rowCount={totalItems}
                loading={loading}
                paginationMode="server"
                paginationModel={paginationModel}
                onPaginationModelChange={(model) => {
                    setPaginationModel(model);

                    // 0-based -> 1-based
                    fetchPage(model.page + 1, model.pageSize);
                }}
                pageSizeOptions={[5, 10, 20]}
                sx={{
                    height: '700px',
                }}
                disableRowSelectionOnClick
            />
        </Box>
    );
}

export default React.memo(ProductsGrid);
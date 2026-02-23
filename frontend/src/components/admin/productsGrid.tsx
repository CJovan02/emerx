import { DataGrid, type GridColDef } from "@mui/x-data-grid";
import { Box, IconButton } from "@mui/material";
import Inventory2OutlinedIcon from "@mui/icons-material/Inventory2Outlined";
import EditIcon from "@mui/icons-material/Edit";
import DeleteIcon from "@mui/icons-material/Delete";
import { useState } from "react";
import type {ProductResponse} from "../../api/openApi/model";

interface Props {
    data: ProductResponse[];
    totalItems: number;
    loading: boolean;
    fetchPage: (page: number, pageSize: number) => void;
}

export default function ProductsGrid({
                                         data,
                                         totalItems,
                                         loading,
                                         fetchPage
                                     }: Props) {
    const [paginationModel, setPaginationModel] = useState({
        page: 0,
        pageSize: 10
    });

    const columns: GridColDef[] = [
        {
            field: "image",
            headerName: "",
            width: 70,
            filterable: false,
            align: "center",
            renderCell: () => <Inventory2OutlinedIcon />
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
            field: "actions",
            headerName: "Actions",
            width: 120,
            sortable: false,
            renderCell: (params) => (
                <>
                    <IconButton
                        onClick={(e) => {
                            e.stopPropagation();
                            console.log("edit", params.row.id);
                        }}
                    >
                        <EditIcon />
                    </IconButton>

                    <IconButton
                        onClick={(e) => {
                            e.stopPropagation();
                            console.log("delete", params.row.id);
                        }}
                    >
                        <DeleteIcon />
                    </IconButton>
                </>
            )
        }
    ];

    return (
        <Box sx={{ height: 700, width: "100%" }}>
            <DataGrid
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
            />
        </Box>
    );
}
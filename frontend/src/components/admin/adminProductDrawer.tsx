import {Box, Divider, Drawer, Toolbar, Typography} from "@mui/material";
import {Inventory} from "@mui/icons-material";
import type {ProductResponse} from "../../api/openApi/model";
import useAdminProductDrawerLogic from "../../hooks/pageLogic/useAdminProductDrawerLogic.ts";

type Props = {
    open: boolean;
    handleClose: () => void;
    product:  ProductResponse | null;
}

export default function AdminProductDrawer({open, handleClose, product}: Props) {
    const {  } = useAdminProductDrawerLogic(product);

    return (
        <Drawer
            open={open}
            onClose={handleClose}
            anchor='right'
            variant='temporary'
        >
            <Box width='500px'>
                <Toolbar
                    sx={{
                        display: 'flex',
                        alignItems: 'center',
                        gap: 3
                    }}
                >
                    <Inventory color='primary' fontSize='large'/>
                    <Typography variant='h6' color='primary' fontWeight={700}>
                        Edit "{product?.name}"
                    </Typography>
                </Toolbar>
                <Divider/>

            </Box>
        </Drawer>
    )

}
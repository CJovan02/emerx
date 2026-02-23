import {Container} from "@mui/material";
import ProductsGrid from "../../components/admin/productsGrid.tsx";
import {mockProducts} from "../../api/mock/mockProducts.ts";

export default function AdminProductsPage() {
    return (
        <Container>
            <ProductsGrid data={mockProducts} totalItems={1000} loading={false} fetchPage={() => {}} />
        </Container>
    )
}
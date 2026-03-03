import { useEffect, useState } from 'react';
import { useProductGetPaged } from '../../api/openApi/product/product.ts';
import { useProductFiltersStore } from '../../stores/productFiltersStore.ts';

const PAGE_SIZE = 12;

function useProductsPageLogic() {
	const [page, setPage] = useState(1);

	const { search, category, minPrice, maxPrice, minRating, inStockOnly } =
		useProductFiltersStore();

useEffect(() => {
		setPage(1);
	}, [search, category, minPrice, maxPrice, minRating, inStockOnly]);

	const { data, isPending, isFetching, isError, refetch } = useProductGetPaged({
		Page: page,
		PageSize: PAGE_SIZE,
		Search: search || undefined,
		Category: category ?? undefined,
		MinPrice: minPrice ?? undefined,
		MaxPrice: maxPrice ?? undefined,
		MinRating: minRating ?? undefined,
		InStockOnly: inStockOnly || undefined,
	});

	return {
		products: data?.items ?? [],
		isLoading: isPending || isFetching,
		isError,
		refetch,
		page,
		setPage,
		totalPages: data?.totalPages ?? 1,
	};
}

export default useProductsPageLogic;

const adminGetProductsPagedName = 'adminGetProductsPaged';

export const QueryKeys = {
	adminGetProductsPagedAll: [adminGetProductsPagedName],
	adminGetProductsPaged: (page: number, pageSize: number, search: string) =>
		[adminGetProductsPagedName, { page, pageSize, search}] as const,
};

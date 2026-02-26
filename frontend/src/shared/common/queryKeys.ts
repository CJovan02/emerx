export const QueryKeys = {
	adminGetProductsPaged: (page: number, pageSize: number) =>
		['adminGetProductsPaged', { page, pageSize }] as const,
};

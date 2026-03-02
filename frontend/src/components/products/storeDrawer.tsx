import {
	Box,
	Button,
	Checkbox,
	Divider,
	Drawer,
	FormControlLabel,
	InputAdornment,
	List,
	ListSubheader,
	Rating,
	Skeleton,
	Switch,
	TextField,
	Toolbar,
	Typography,
} from '@mui/material';
import { useStoreDrawerStore } from '../../stores/storeDrawerStore.tsx';
import { Drawers } from '../../shared/common/constants/drawers.ts';
import useScreenSize from '../../hooks/useScreenSize.ts';
import {
	hasActiveFilters,
	useProductFiltersStore,
} from '../../stores/productFiltersStore.ts';
import { useProductGetCategories } from '../../api/openApi/product/product.ts';

const drawerWidth = Drawers.Store.Width;

export default function StoreDrawer() {
	const { isDesktop } = useScreenSize();
	const close = useStoreDrawerStore(state => state.close);
	const isOpen = useStoreDrawerStore(state => state.isOpen);

	const {
		category,
		minPrice,
		maxPrice,
		minRating,
		inStockOnly,
		setCategory,
		setMinPrice,
		setMaxPrice,
		setMinRating,
		setInStockOnly,
		clearFilters,
	} = useProductFiltersStore();

	const filtersActive = useProductFiltersStore(hasActiveFilters);

	const { data: categories, isPending: categoriesLoading } =
		useProductGetCategories();

	return (
		<Drawer
			open={isDesktop ? true : isOpen}
			onClose={close}
			variant={isDesktop ? 'permanent' : 'temporary'}
			sx={{
				width: drawerWidth,
				flexShrink: 0,
				'& .MuiDrawer-paper': {
					width: drawerWidth,
					boxSizing: 'border-box',
				},
			}}
			anchor='left'>
			<Toolbar />
			<Divider />

			<Box sx={{ overflow: 'auto', px: 2, py: 1 }}>
				<Box
					sx={{
						display: 'flex',
						alignItems: 'center',
						justifyContent: 'space-between',
						mb: 1,
					}}>
					<Typography variant='subtitle1' fontWeight={700}>
						Filters
					</Typography>
					{filtersActive && (
						<Button
							size='small'
							color='error'
							variant='text'
							onClick={clearFilters}>
							Clear all
						</Button>
					)}
				</Box>

				<Divider sx={{ mb: 2 }} />

				<List
					dense
					disablePadding
					subheader={
						<ListSubheader disableGutters sx={{ lineHeight: '32px' }}>
							Category
						</ListSubheader>
					}>
					{categoriesLoading
						? [1, 2, 3].map(i => (
								<Skeleton key={i} variant='text' height={32} sx={{ mb: 0.5 }} />
							))
						: (categories ?? []).map(cat => (
								<FormControlLabel
									key={cat}
									label={cat}
									sx={{ display: 'flex', ml: 0 }}
									control={
										<Checkbox
											checked={category === cat}
											onChange={() =>
												setCategory(category === cat ? null : cat)
											}
											size='small'
										/>
									}
								/>
							))}
				</List>

				<Divider sx={{ my: 2 }} />

				<Typography variant='body2' fontWeight={600} gutterBottom>
					Price range
				</Typography>
				<Box sx={{ display: 'flex', gap: 1 }}>
					<TextField
						label='Min'
						type='number'
						size='small'
						value={minPrice ?? ''}
						onChange={e =>
							setMinPrice(e.target.value === '' ? null : Number(e.target.value))
						}
						slotProps={{
							input: {
								startAdornment: (
									<InputAdornment position='start'>$</InputAdornment>
								),
							},
						}}
					/>
					<TextField
						label='Max'
						type='number'
						size='small'
						value={maxPrice ?? ''}
						onChange={e =>
							setMaxPrice(e.target.value === '' ? null : Number(e.target.value))
						}
						slotProps={{
							input: {
								startAdornment: (
									<InputAdornment position='start'>$</InputAdornment>
								),
							},
						}}
					/>
				</Box>

				<Divider sx={{ my: 2 }} />

				<Typography variant='body2' fontWeight={600} gutterBottom>
					Minimum rating
				</Typography>
				<Rating
					value={minRating}
					onChange={(_, value) => setMinRating(value)}
				/>
				{minRating !== null && (
					<Button
						size='small'
						variant='text'
						sx={{ mt: 0.5, p: 0 }}
						onClick={() => setMinRating(null)}>
						Any rating
					</Button>
				)}

				<Divider sx={{ my: 2 }} />

				<FormControlLabel
					label='In stock only'
					control={
						<Switch
							checked={inStockOnly}
							onChange={e => setInStockOnly(e.target.checked)}
							size='small'
						/>
					}
				/>
			</Box>
		</Drawer>
	);
}

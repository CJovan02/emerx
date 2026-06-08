import SearchIcon from '@mui/icons-material/Search';
import { InputAdornment, TextField } from '@mui/material';

interface SearchBarProps {
	value: string;
	onChange: (value: string) => void;
	placeholder?: string;
	fullWidth?: boolean;
}

export default function SearchBar({
																		value,
																		onChange,
																		placeholder = 'Search...',
																		fullWidth
																	}: SearchBarProps) {
	return (
		<TextField
			fullWidth={fullWidth}
			size="small"
			value={value}
			placeholder={placeholder}
			onChange={e => onChange(e.target.value)}
			slotProps={{
				htmlInput: {
					'data-testid': 'admin-search-bar',
				},
				input: {
					startAdornment: (
						<InputAdornment position="start">
							<SearchIcon />
						</InputAdornment>
					),
				},
			}}
		/>
	);
}
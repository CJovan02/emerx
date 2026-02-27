import {Box} from "@mui/material";
import {BrokenImage} from "@mui/icons-material";

export default function ProductDetailImage({thumbnailUrl, name}: {thumbnailUrl: string | null | undefined, name: string}) {
    return (
			<>
				{thumbnailUrl && (
					<Box
						component='img'
						src={thumbnailUrl!}
						alt={name}
						sx={{
							width: '100%',
							borderRadius: 3,
							objectFit: 'cover',
							aspectRatio: '1 / 1',
							boxShadow: 3,
						}}
					/>
				)}
				{!thumbnailUrl && (
					<Box
						sx={{
							width: '100%',
							aspectRatio: '1 / 1',
							borderRadius: 3,
							bgcolor: 'grey.100',
							display: 'flex',
							alignItems: 'center',
							justifyContent: 'center',
							boxShadow: 1,
						}}>
						<BrokenImage sx={{ fontSize: 48, color: 'grey.400' }} />
					</Box>
				)}
			</>
		);   
}
import { Box, Dialog } from '@mui/material';
import { useState } from 'react';

type Props = {
	src: string;
	alt: string;
	width?: number;
	height?: number;
	borderRadius?: number;
	aspectRatio?: number;
	boxShadow?: number;
	disableCursorPointer?: boolean;
};

function ImageLightbox({
	src,
	alt,
	aspectRatio,
	boxShadow,
	height,
	borderRadius,
	width,
	disableCursorPointer,
}: Props) {
	const [open, setOpen] = useState(false);

	return (
		<>
			<Box
				component='img'
				src={src}
				alt={alt}
				onClick={() => setOpen(true)}
				style={{ transform: open ? 'scale(1.05)' : undefined }}
				sx={{
					width: width ?? '100%',
					height: height,
					borderRadius: borderRadius,
					objectFit: 'cover',
					aspectRatio: aspectRatio ?? '1/1',
					boxShadow: boxShadow,
					cursor: !disableCursorPointer ? 'pointer' : null,
					transition: 'transform 0.2s ease-in-out',
				}}
			/>

			<Dialog
				open={open}
				onClose={() => setOpen(false)}
				maxWidth='md'>
				<Box
					component='img'
					src={src}
					alt={alt}
					sx={{
						width: '100%',
						height: 'auto',
					}}
				/>
			</Dialog>
		</>
	);
}

export default ImageLightbox;

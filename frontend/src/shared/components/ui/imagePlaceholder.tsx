import {Box} from "@mui/material";
import {BrokenImage} from "@mui/icons-material";

type Props = {
    fontSize?: number;
    width?: number;
    height?: number;
    borderRadius?: number;
}

export default function ImagePlaceholder({borderRadius, fontSize, width, height}: Props) {
    return (
        <Box
            sx={{
                width: width ?? '100%',
                height: height,
                aspectRatio: '1 / 1',
                borderRadius: borderRadius ?? 3,
                bgcolor: 'grey.100',
                display: 'flex',
                alignItems: 'center',
                justifyContent: 'center',
                boxShadow: 1,
            }}>
            <BrokenImage sx={{ fontSize: fontSize, color: 'grey.400' }} />
        </Box>
    );
}
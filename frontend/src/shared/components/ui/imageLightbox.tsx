import {Box, Dialog, styled} from "@mui/material";
import {useState} from "react";

const ProductImage = styled('img')({
    width: 60,
    height: 60,
    objectFit: 'cover',
    borderRadius: 12,
    cursor: 'pointer',
    transition: 'transform 0.2s ease',
});

function ImageLightbox({src, alt}: { src: string, alt: string }) {
    const [open, setOpen] = useState(false);

    return (
        <>
            <ProductImage
                src={src}
                onClick={() => setOpen(true)}
                style={{transform: open ? 'scale(1.05)' : undefined}}
            />

            <Dialog
                open={open}
                onClose={() => setOpen(false)}
                maxWidth="md"
            >
                <Box
                    component="img"
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
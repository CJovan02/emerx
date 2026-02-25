import {useCallback, useEffect, useState} from "react";
import {useDropzone} from "react-dropzone";
import {Box, Typography, IconButton} from "@mui/material";
import DeleteIcon from "@mui/icons-material/Delete";
import CloudUploadIcon from "@mui/icons-material/CloudUpload";
import {useSnackbar} from "notistack";
import imageCompression from "browser-image-compression";

type Props = {
    // File object is when we upload the file through web
    // string object is when we get back the imageUrl from the server
    value: File | string | null;

    // This function is used to notify the parent component that image was changed
    // this way the component stays stateless
    onChange: (value: File | null) => void;

    id?: string;

    width?: number;
    height?: number;
};

// Dropzone currently supports one image
// Parent is the one holding the image value object, this component is pretty much stateless
export default function ProductImageDropzone({value, onChange, width, height, id}: Props) {
    // When we upload a file object we need to generate url in order to show the image
    // this state is used for that.
    // If the 'value' prop is the url than this stays null, since it's only used when 'value' is File type
    const [objectUrl, setObjectUrl] = useState<string | null>(null);

    const {enqueueSnackbar} = useSnackbar();

    useEffect(() => {
        if (value instanceof File) {
            const url = URL.createObjectURL(value);
            setObjectUrl(url);

            return () => {
                URL.revokeObjectURL(url);
            }
        } else {
            setObjectUrl(null);
        }
    }, [value]);

    // Real variable used for rendering the image preview
    // If the value provided is URL, we just use that
    // If the value provided is the FILE type, we use the generated objectUrl
    const previewUrl =
        value instanceof File
            ? objectUrl
            : typeof value === "string"
                ? value
                : null;

    const onDrop = useCallback(async (acceptedFiles: File[]) => {
        if (acceptedFiles.length > 0) {
            const file = acceptedFiles[0];
            const originalSizeMb = file.size / 1024 / 1024;
            console.log(`Original image size: ${originalSizeMb.toFixed(3)}MB`);

            // we notify the parent about the selected File
            const compressedFile = await compressFile(file)
            const compressedFileSizeMb = compressedFile.size / 1024 / 1024
            console.log(`compressed file size: ${compressedFileSizeMb.toFixed(3)}MB`);
            console.log(`Total reduction is: ${((originalSizeMb - compressedFileSizeMb) * 1024).toFixed(3)}KB`);

            onChange(compressedFile);
        }
    }, [onChange]);

    async function compressFile(file: File) {
        const options = {
            maxSizeMB: 1,
            maxWidthOrHeight: 1920,
            useWebWorker: true,
        }
        return await imageCompression(file, options);
    }

    const handleRemove = () => {
        onChange(null);
    }

    const {getRootProps, getInputProps, isDragActive} = useDropzone({
        onDrop,
        accept: {
            "image/*": []
        },
        maxFiles: 1,
        multiple: false,
        maxSize: 1024 * 1024,
        onDropRejected: (fileRejections) => {
            fileRejections.forEach(({file, errors}) => {
                errors.forEach(e => enqueueSnackbar(`File ${file.name} rejected: ${e.message}`, {variant: 'error'}));
            });
        }
    });

    return (
        <Box display='flex' justifyContent='center'>
            {!previewUrl && (
                <Box
                    {...getRootProps()}
                    sx={{
                        border: "2px dashed",
                        borderColor: isDragActive ? "primary.main" : "grey.400",
                        borderRadius: 2,
                        p: 4,
                        textAlign: "center",
                        cursor: "pointer",
                        transition: "0.2s",
                        "&:hover": {
                            borderColor: "primary.main"
                        }
                    }}
                >
                    <input id={id} {...getInputProps()} />
                    <CloudUploadIcon sx={{fontSize: 40, mb: 1}}/>
                    <Typography variant="body1">
                        {isDragActive
                            ? "Drop the image here..."
                            : "Drag & drop image here, or click to select"}
                    </Typography>
                </Box>
            )}

            {previewUrl && (
                <Box
                    sx={{
                        position: "relative",
                        width: width ?? 250,
                        height: height ?? 250,
                        borderRadius: 2,
                        overflow: "hidden",
                        border: "1px solid",
                        borderColor: "grey.300",
                    }}
                >
                    <img
                        src={previewUrl}
                        alt="Product preview"
                        style={{
                            width: "100%",
                            height: "100%",
                            objectFit: "cover"
                        }}
                    />

                    <IconButton
                        onClick={handleRemove}
                        sx={{
                            position: "absolute",
                            top: 8,
                            right: 8,
                            backgroundColor: "rgba(0,0,0,0.6)",
                            color: "white",
                            "&:hover": {
                                backgroundColor: "rgba(0,0,0,0.8)"
                            }
                        }}
                    >
                        <DeleteIcon/>
                    </IconButton>
                </Box>
            )}
        </Box>
    );
}
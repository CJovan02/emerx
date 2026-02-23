import Rating from "@mui/material/Rating";
import * as React from "react";

interface Props {
    value: number | undefined;
}

// Memoized version of rating MUI component for faster performance
const RatingMemo = React.memo(({value}: Props) => {
    return <Rating value={value} precision={0.5} size="small" readOnly />;
});

export default RatingMemo;
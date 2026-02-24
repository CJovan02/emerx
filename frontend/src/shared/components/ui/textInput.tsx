import {Controller, useFormContext} from 'react-hook-form';
import TextField from "@mui/material/TextField";

interface TextInputProps {
    id: string;
    label?: string;
    description?: string;
    placeholder?: string;
    type?: string;
    required?: boolean;
    disabled?: boolean;
    fullWidth?: boolean;
}

const TextInput = ({
                       id,
                       label,
                       placeholder,
                       required,
                       disabled,
                       type,
                       description,
                       fullWidth
                   }: TextInputProps) => {
    const {control} = useFormContext();
    return (
        <Controller
            name={id}
            control={control}
            render={({field, fieldState}) => {
                const {ref, ...fieldRest} = field;
                return (
                    <>
                        <TextField
                            id={id}
                            required={required}
                            disabled={disabled}
                            ref={ref}
                            placeholder={placeholder}
                            {...fieldRest}
                            value={field.value ?? ''}
                            onChange={(event) => {
                                const value = event.target.value;
                                field.onChange(type === 'number' ? Number(value) : value);
                            }}
                            label={label}
                            fullWidth={fullWidth}
                            type={type || 'text'}
                            error={fieldState.invalid}
                            helperText={fieldState.invalid ? fieldState.error?.message : description}
                        />
                    </>
                );
            }}
        />
        // <Controller
        //     name={id}
        //     control={control}
        //     render={({ field, fieldState }) => {
        //         const { ref, ...fieldRest } = field;
        //         return (
        //             <Field data-invalid={fieldState.invalid}>
        //                 {label && <FieldLabel htmlFor={field.name}>{label}</FieldLabel>}
        //                 <Input
        //                     id={id}
        //                     required={required}
        //                     disabled={disabled}
        //                     ref={ref}
        //                     maxLength={maxLength}
        //                     placeholder={placeholder}
        //                     {...fieldRest}
        //                     aria-invalid={fieldState.invalid}
        //                     autoComplete='off'
        //                     value={field.value ?? ''}
        //                     onChange={(event) => {
        //                         const value = event.target.value;
        //                         field.onChange(type === 'number' ? Number(value) : value);
        //                     }}
        //                     type={type || 'text'}
        //                     className={inputClassName}
        //                 />
        //                 {description !== undefined && (
        //                     <FieldDescription>{description}</FieldDescription>
        //                 )}
        //                 {fieldState.invalid && <FieldError errors={[fieldState.error]} />}
        //             </Field>
        //         );
        //     }}
        // />
    );
};

export default TextInput;
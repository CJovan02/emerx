import z from 'zod';
import {useForm} from "react-hook-form";
import {useCallback} from "react";
import {zodResolver} from "@hookform/resolvers/zod";

function useLoginLogic() {
    const formSchema = z.object({
        email: z.string().min(5, "Min is 5"),
        password: z.string(),
    })
    type FormValues = z.infer<typeof formSchema>;

    const form = useForm<FormValues>({
        resolver: zodResolver(formSchema),
        defaultValues: {
            email: '',
            password: ''
        }
    });

    const login = useCallback((values: FormValues) => {
        console.log(values);
    }, [])

    return {form, login}
}

export default useLoginLogic;
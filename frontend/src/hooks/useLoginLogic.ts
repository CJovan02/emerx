import z from 'zod';
import {useForm} from "react-hook-form";
import {useCallback, useState} from "react";
import {zodResolver} from "@hookform/resolvers/zod";
import {auth} from "../config/firebase.ts";
import {signInWithEmailAndPassword} from "firebase/auth"
import {FirebaseError} from "firebase/app"

type LoginLogicState = 'init' | 'success' | 'error' | 'loading';

function useLoginLogic() {
    const formSchema = z.object({
        email: z.email(),
        password: z.string().min(6, "Password must be at least 6 characters long."),
    })
    type FormValues = z.infer<typeof formSchema>;

    const form = useForm<FormValues>({
        resolver: zodResolver(formSchema),
        defaultValues: {
            email: '',
            password: ''
        }
    });

    const [state, setState] = useState<LoginLogicState>('init')
    const [errorMessage, setErrorMessage] = useState<string>('')

    const login = useCallback(async (values: FormValues) => {
        try {
            if (state === 'loading') return;
            setState('loading');

            const credential = await signInWithEmailAndPassword(auth, values.email, values.password)
            console.log(credential)

            setState('success')
        } catch (error) {
            setState('error')
            if (error instanceof FirebaseError && error.code === 'auth/invalid-credential') {
                setErrorMessage("Invalid email or password");
                return;
            }

            setErrorMessage("Unexpected error happened")
            console.error(error)
        }
    }, [auth, signInWithEmailAndPassword, state, setState, setErrorMessage])

    return {
        form,
        login,
        isLoading: state === 'loading',
        isSuccess: state === 'success',
        isError: state === 'error',
        errorMessage,
    }
}

export default useLoginLogic;
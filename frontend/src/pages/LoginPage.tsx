import {Button, Container} from "@mui/material";
import TextInput from "../componenets/reusable/textInput.tsx";
import useLoginLogic from "../hooks/useLoginLogic.ts";
import {FormProvider} from "react-hook-form";

const LoginPage = () => {
    const {form, login} = useLoginLogic();

    return (
        <Container>
            <FormProvider {...form}>
                <form
                    id='login-form'
                    onSubmit={form.handleSubmit(login)}
                >
                    <TextInput
                        id='email'
                        label='Email'
                        required
                    />
                </form>
            </FormProvider>
            <Button type='submit' form='login-form'>Login</Button>

        </Container>
    )
};

export default LoginPage;

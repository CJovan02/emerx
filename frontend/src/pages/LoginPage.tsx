import {
    alpha,
    Box,
    Button,
    Card,
    CardActions,
    CardContent,
    CardHeader,
    Link,
    Stack,
} from "@mui/material";
import TextInput from "../componenets/reusable/textInput.tsx";
import useLoginLogic from "../hooks/useLoginLogic.ts";
import {FormProvider} from "react-hook-form";

const LoginPage = () => {
    const {form, login} = useLoginLogic();

    return (
        <Box
            minHeight="100vh"
            display="flex"
            alignItems="center"
            justifyContent="center"
            sx={{
                background: (theme) =>
                    `linear-gradient(
        135deg,
        ${alpha(theme.palette.primary.main, 0.08)},
        ${alpha(theme.palette.secondary.main, 0.08)}
      )`
            }}
        >
            <Card
                elevation={3}
                sx={{
                    padding: 2
                }}
            >
                <CardHeader
                    title="Sign In"
                    subheader="Welcome back, please login to start shopping"
                />
                <CardContent>
                    <FormProvider {...form}>
                        <form
                            id='login-form'
                            onSubmit={form.handleSubmit(login)}
                        >
                            <Stack gap={3}>
                                <TextInput
                                    id='email'
                                    label='Email'
                                    required
                                    fullWidth
                                />

                                <TextInput
                                    id='password'
                                    label='Password'
                                    type='password'
                                    required
                                    fullWidth
                                />
                            </Stack>
                        </form>
                    </FormProvider>

                    <Box marginY={2}>Don't have an account? <Link href='#'>Sign up</Link></Box>
                </CardContent>
                <CardActions>
                    <Button
                        type="submit"
                        form="login-form"
                        fullWidth
                        size='large'
                        variant="contained"
                        sx={{
                            py: 1.5,
                        }}
                    >
                        Sign In
                    </Button>
                </CardActions>
            </Card>

        </Box>
    )
};

export default LoginPage;

import { useEffect } from 'react';
import { auth } from '../../config/firebase.ts';
import { useUserStore } from '../../stores/userStore.ts';
import { userGetSelf } from '../../api/openApi/user/user.ts';
import { Backdrop } from '@mui/material';
import CircularProgress from '@mui/material/CircularProgress';
import { useSnackbar } from 'notistack';
import { mapResponseToUser } from '../../domain/models/appUser.ts';
import { useCartStore } from '../../stores/cartStore.ts';

// This component is used to sync firebase user state changes (sign in/sign out) with the user store
// AppUser signs in -> call the server and fill in user store
// AppUser signs out -> clear user store

// Also I use it to manually hydrate the cartStore with local storage
function AuthUserSync() {
	const setUser = useUserStore(state => state.setUser);
	const clearUser = useUserStore(state => state.clearUser);
	const isLoading = useUserStore(state => state.isLoading);
	const setLoading = useUserStore(state => state.setLoading);

	const { enqueueSnackbar } = useSnackbar();

	const clearCart = useCartStore(state => state.clearCart);

	useEffect(() => {
		const unsubscribe = auth.onAuthStateChanged(async user => {
			const prefix = user ? user.uid : 'anonymous';
			useCartStore.persist.setOptions({
				name: `${prefix}-cart-storage`,
			});
			await useCartStore.persist.rehydrate();
			// This is important. We always need an 'anonymous-cart-storage' local storage item, even if it's empty.
			// After hydrating and if user is null (it means we logged out we are using 'anonymous-cart-storage') it will automatically change the cartStore() to
			// whatever was inside this anonymous one, and after changing account it will pick up the 'right' local storage item
			// This just forces the creation of 'anonymous' storage key when the app starts. Without this the store will never clear out when
			// we log out.
			if (!user) clearCart();

			if (!user) {
				console.log('AppUser signed out');
				clearUser();
				return;
			}
			console.log('AppUser signed in');

			try {
				setLoading(true);

				// We directly call the endpoint, react query for this situation complicates stuff
				const userResponse = await userGetSelf();

				// Now we extract user roles from firebase jwt
				// Backend can also do this logic, but I would like to avoid sending roles information through REST
				const tokenResult = await user.getIdTokenResult();
				// console.log(tokenResult.token);
				const roles: number[] = (tokenResult.claims.roles as number[]) ?? [];
				const appUser = mapResponseToUser(userResponse, roles);

				setUser(appUser);
			} catch (error) {
				enqueueSnackbar('Error trying to retrieve your user data', {
					variant: 'error',
				});

				// This will reset user store, it's written 10 lines above :)
				await auth.signOut();
				console.error(error);
			} finally {
				setLoading(false);
			}
		});

		return () => unsubscribe();
	}, [clearUser, enqueueSnackbar, setLoading, setUser, clearCart]);

	if (!isLoading) return null;

	return (
		<Backdrop
			sx={theme => ({
				zIndex: theme.zIndex.drawer + 1,
			})}
			open={isLoading}>
			<CircularProgress
				sx={theme => ({ color: theme.palette.primary.contrastText })}
				size={50}
			/>
		</Backdrop>
	);
}

export default AuthUserSync;

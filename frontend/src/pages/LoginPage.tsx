import { useState } from 'react';
import { signInWithEmailAndPassword } from 'firebase/auth';
import { auth } from '../config/firebase.ts';
import {Button} from "@mui/material";
import {useProductGetAll} from "../api/openApi/product/product.ts";

const LoginPage = () => {
	const [email, setEmail] = useState('');
	const [password, setPassword] = useState('');
	const [loading, setLoading] = useState(false);
	const [error, setError] = useState('');

	const { data } = useProductGetAll()
	console.log(data);

	const handleLogin = async (e: React.FormEvent) => {
		e.preventDefault();
		setError('');
		setLoading(true);

		try {
			const userCredential = await signInWithEmailAndPassword(
				auth,
				email,
				password
			);
			const user = userCredential.user;

			// Uzmi token → može i sa force refresh
			const token = await user.getIdToken();

			console.log('Logged in user:', user);
			console.log('Token:', token);

			alert('Login successful!');
		} catch (err: any) {
			setError(err.message ?? 'Unknown error');
		} finally {
			setLoading(false);
		}
	};

	return (
		<div style={{ maxWidth: 400, margin: '50px auto' }}>
			<h2>Login</h2>

			<form onSubmit={handleLogin}>
				<div style={{ marginBottom: '12px' }}>
					<label>Email</label>
					<input
						type='email'
						value={email}
						onChange={e => setEmail(e.target.value)}
						required
						style={{ width: '100%' }}
					/>
				</div>

				<div style={{ marginBottom: '12px' }}>
					<label>Password</label>
					<input
						type='password'
						value={password}
						onChange={e => setPassword(e.target.value)}
						required
						style={{ width: '100%' }}
					/>
				</div>

				{error && <p style={{ color: 'red' }}>{error}</p>}

				<Button
					disabled={loading}
					type='submit'>
					{loading ? 'Logging in...' : 'Login'}
				</Button>
			</form>
		</div>
	);
};

export default LoginPage;

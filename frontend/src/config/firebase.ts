import firebase from 'firebase/compat/app';
import { getAuth } from 'firebase/auth';

const app = firebase.initializeApp({
	apiKey: import.meta.env.VITE_FIREBASE_API_KEY,
	authDomain: import.meta.env.VITE_FIREBASE_AUTH_DOMAIN,
	databaseURL: import.meta.env.VITE_FIREBASE_DATABASE_URL,
	projectId: import.meta.env.VITE_FIREBASE_PROJECT_ID,
	storageBucket: import.meta.env.STORAGE_BUCKET,
	messagingSenderId: import.meta.env.VITE_FIREBASE_MESSAGING_SENDER,
	appId: import.meta.env.VITE_FIREBASE_APP_ID,
});

export const auth = getAuth();
export default app;

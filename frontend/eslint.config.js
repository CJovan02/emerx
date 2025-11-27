import js from '@eslint/js';
import globals from 'globals';
import reactHooks from 'eslint-plugin-react-hooks';
import reactRefresh from 'eslint-plugin-react-refresh';
import tseslint, { parser } from 'typescript-eslint';
import { defineConfig, globalIgnores } from 'eslint/config';
import eslintPluginPrettier from 'eslint-plugin-prettier';

export default defineConfig([
	globalIgnores(['dist']),
	{
		files: ['**/*.{ts,tsx}'],
		extends: [
			js.configs.recommended,
			tseslint.configs.recommended,
			tseslint.configs['recommended-type-checked'],
			reactHooks.configs.flat.recommended,
			reactRefresh.configs.vite,
			eslintPluginPrettier.configs.recommended,
		],
		plugins: {
			prettier: eslintPluginPrettier,
		},
		languageOptions: {
			parser,
			parserOptions: {
				project: './tsconfig.json',
				ecmaVersion: 2023,
				sourceType: 'module',
				ecmaFeatures: { jsx: true },
			},
			globals: globals.browser,
		},
		rules: {
			'no-magic-numbers': 'warn',
			'no-nested-ternary': 'warn',
		},
	},
]);

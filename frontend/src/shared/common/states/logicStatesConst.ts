export const LogicStates = {
	Init: 'init',
	Success: 'success',
	Error: 'error',
	Loading: 'loading',
} as const;

export type LogicState = (typeof LogicStates)[keyof typeof LogicStates];

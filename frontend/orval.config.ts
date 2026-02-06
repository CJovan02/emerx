import {defineConfig} from 'orval';

export default defineConfig(() => {
    return {
        api: {
            input: 'http://localhost:5016/swagger/v1/swagger.json',
            output: {
                mode: 'tags-split',
                target: './src/api/openApi/endpoints.ts',
                schemas: './src/api/openApi/model',
                clean: true,
                prettier: true,
                client: 'react-query',
            },
            hooks: {
                afterAllFilesWrite: 'prettier --write',
            },
        },
    }
})

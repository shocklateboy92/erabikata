import type { ConfigFile } from '@rtk-query/codegen-openapi';

const config: ConfigFile = {
    schemaFile: '../backend/Erabikata.Backend/obj/swagger.json',
    apiFile: './src/emptyApi.ts',
    outputFile: './src/backend-rtk.generated.ts',
    hooks: true
};

export default config;

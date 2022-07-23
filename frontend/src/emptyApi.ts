import { createApi, fetchBaseQuery } from '@reduxjs/toolkit/dist/query';
import { getInitialBaseUrl } from 'features/backendSelection';

export const api = createApi({
    baseQuery: fetchBaseQuery({ baseUrl: getInitialBaseUrl() }),
    endpoints: () => ({})
});

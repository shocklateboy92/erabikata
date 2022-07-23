import { createApi, fetchBaseQuery } from '@reduxjs/toolkit/query/react';
import { getInitialBaseUrl } from 'features/backendSelection';

export const api = createApi({
    baseQuery: fetchBaseQuery({ baseUrl: getInitialBaseUrl() }),
    endpoints: () => ({})
});

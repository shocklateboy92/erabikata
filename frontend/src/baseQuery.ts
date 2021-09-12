import { fetchBaseQuery } from '@reduxjs/toolkit/query';
import { getInitialBaseUrl } from './features/backendSelection';

export const baseQuery = fetchBaseQuery({
    baseUrl: getInitialBaseUrl()
});

export default baseQuery;

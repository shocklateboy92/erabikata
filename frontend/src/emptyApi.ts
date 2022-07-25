import { createApi, fetchBaseQuery } from '@reduxjs/toolkit/query/react';
import { getInitialBaseUrl } from 'features/backendSelection';

export const api = createApi({
    baseQuery: fetchBaseQuery({
        baseUrl: getInitialBaseUrl(),
        // ASP.NET expects arrays to be split into multiple params
        paramsSerializer: (params) => {
            const serializer = new URLSearchParams();
            Object.entries(params).forEach(([param, value]) => {
                if (Array.isArray(value)) {
                    value.forEach((v) => serializer.append(param, v));
                } else if (value !== undefined) {
                    serializer.append(param, value);
                }
            });

            return serializer.toString();
        }
    }),
    endpoints: () => ({})
});

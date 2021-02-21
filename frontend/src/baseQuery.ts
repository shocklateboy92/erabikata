import { fetchBaseQuery } from '@rtk-incubator/rtk-query';
import { getInitialBaseUrl } from './features/backendSelection';

export const baseQuery = (args: object) => {
    return fetchBaseQuery({
        ...args,
        baseUrl: getInitialBaseUrl()
    });
};

export default baseQuery;

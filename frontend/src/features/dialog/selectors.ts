import { RootState } from 'app/rootReducer';
import { apiEndpoints } from '../../backend';

export const selectEpisodeTitle = (
    state: RootState,
    episodeId: string | null
) => {
    if (!episodeId) {
        return;
    }
    return apiEndpoints.episodeIndex.select({ episodeId })(state).data?.title;
};

export const selectNearbyValues = (
    array: number[] | undefined,
    value: number,
    count: number
) => {
    const index = array?.findIndex((d) => d > value);
    if (index !== undefined && index >= 0) {
        return array!.slice(Math.max(0, index - count), index + count);
    }

    return [];
};

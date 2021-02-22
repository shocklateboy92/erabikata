import { RootState } from 'app/rootReducer';
import { selectAnalyzer } from 'features/backendSelection';
import { apiEndpoints } from '../../backend';

export const selectEpisodeTitle = (
    state: RootState,
    episodeId: string | null
) => {
    if (!episodeId) {
        return;
    }
    const analyzer = selectAnalyzer(state);
    return apiEndpoints.episodeIndex.select({ analyzer, episodeId })(state).data
        ?.title;
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

import { useEpisodeIndexQuery } from 'backend';
import { findNearbyDialog } from 'features/selectedWord';

export const useNearbyDialogQuery = (
    episodeId: string,
    time: number,
    count: number
) => {
    const response = useEpisodeIndexQuery({ episodeId });

    const dialog = !!response.data
        ? findNearbyDialog(response.data.entries, time, count)
        : [];

    return { response, dialog };
};

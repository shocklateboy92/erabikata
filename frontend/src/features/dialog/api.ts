import { useTypedSelector } from 'app/hooks';
import { useEpisodeIndexQuery } from 'backend';
import { selectAnalyzer } from 'features/backendSelection';
import { findNearbyDialog } from 'features/selectedWord';

export const useNearbyDialogQuery = (
    episodeId: string,
    time: number,
    count: number
) => {
    const analyzer = useTypedSelector(selectAnalyzer);
    const response = useEpisodeIndexQuery({ analyzer, episodeId });

    const dialog =
        response.data && findNearbyDialog(response.data.entries, time, count);

    return { response, dialog };
};

import { RootState } from 'app/rootReducer';
import { apiEndpoints } from 'backend';
import { selectSelectedWord } from 'features/selectedWord';

export const selectSelectedEnglishDialog = (state: RootState) => {
    const { episode: episodeId, sentenceTimestamp: time } = selectSelectedWord(
        state
    );
    if (!episodeId || !time) {
        return;
    }

    const { data } = apiEndpoints.engSubsIndex.select({
        episodeId,
        time,
        count: 3
    })(state);
    if (!data) {
        return;
    }

    return data.dialog[Math.floor(data.dialog.length / 2)];
};

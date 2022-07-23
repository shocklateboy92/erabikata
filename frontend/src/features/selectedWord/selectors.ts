import { RootState } from 'app/rootReducer';
import { apiEndpoints } from '../../backend';
import { Entry } from 'backend.generated';
import { notUndefined } from '../../typeUtils';
import { IEpisodeTime } from 'features/anki/ankiSlice';

export const selectIsCurrentlySelected = (
    state: RootState,
    episodeId?: string,
    time?: number
) =>
    state.selectedWord.episode === episodeId &&
    state.selectedWord.sentenceTimestamp === time;

export const selectSelectedEpisodeContent = (state: RootState) => {
    const { episode } = state.selectedWord;
    if (!episode) {
        return;
    }
    return apiEndpoints.episodeIndex.select({ episodeId: episode })(state).data
        ?.entries;
};

export const selectSelectedWord = (state: RootState) => state.selectedWord;

export const selectSelectedEpisodeId = (state: RootState) =>
    state.selectedWord.episode;

export const selectSelectedEpisodeTime = ({
    selectedWord: { episode, sentenceTimestamp }
}: RootState) => {
    if (episode && sentenceTimestamp) {
        return { episodeId: episode, time: sentenceTimestamp };
    }
};

export const selectSelectedWords = (state: RootState) =>
    state.selectedWord.wordIds;

export const shouldShowPanel = ({ selectedWord }: RootState) =>
    selectedWord.wordIds.length > 0 ||
    (selectedWord.episode && selectedWord.sentenceTimestamp);

export const selectSelectedWordOccurrences = (state: RootState) => {
    const {
        wordIds: [wordId]
    } = state.selectedWord;
    if (!wordId) {
        return;
    }
    const dialogIds = apiEndpoints.wordsOccurrences.select({
        wordId
    })(state).data?.dialogIds;
    if (!dialogIds?.length) {
        return;
    }

    return dialogIds
        .map((id) => apiEndpoints.subsById.select({ id })(state).data)
        .filter(notUndefined);
};

export const selectNearestSelectedDialog = (state: RootState) => {
    if (state.selectedWord.episode && state.selectedWord.sentenceTimestamp) {
        return selectNearestDialogContent(state, {
            episodeId: state.selectedWord.episode,
            time: state.selectedWord.sentenceTimestamp
        });
    }
};

export const selectNearestDialogContent = (
    state: RootState,
    { episodeId, time }: IEpisodeTime
) => {
    const { data } = apiEndpoints.episodeIndex.select({
        episodeId: episodeId
    })(state);
    if (!data) {
        return;
    }

    const [{ dialogId }] = findNearbyDialog(data.entries, time, 1);
    return apiEndpoints.subsById.select({ id: dialogId })(state).data?.text;
};

export function findNearbyDialog(
    dialog: Entry[],
    timeOverride: number,
    count: number
) {
    const match = binarySearch(dialog, timeOverride);
    const index = Math.max(0, match - count + 1);
    return dialog.slice(index, index + count * 2 - 1);
}

function binarySearch(
    arr: Entry[],
    target: number,
    lo = 0,
    hi = arr.length - 1
): number {
    if (target < arr[lo].time) {
        return 0;
    }
    if (target > arr[hi].time) {
        return hi;
    }

    const mid = Math.floor((hi + lo) / 2);

    if (hi - lo < 2) {
        return target - arr[lo].time < arr[hi].time - target ? lo : hi;
    } else {
        return target < arr[mid].time
            ? binarySearch(arr, target, lo, mid)
            : target > arr[mid].time
            ? binarySearch(arr, target, mid, hi)
            : mid;
    }
}

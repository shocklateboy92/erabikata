import { RootState } from 'app/rootReducer';

export const selectIsCurrentlySelected = (
    state: RootState,
    episodeId: string,
    time: number
) =>
    state.selectedWord.episode === episodeId &&
    state.selectedWord.sentenceTimestamp === time;

export const selectSelectedEpisodeContent = (state: RootState) =>
    state.dialog.order[state.selectedWord.episode!];

export const selectSelectedWord = (state: RootState) => state.selectedWord;

export const selectSelectedEpisodeId = (state: RootState) =>
    state.selectedWord.episode;

export const selectSelectedWords = (state: RootState) =>
    state.selectedWord.wordIds;

export const shouldShowPanel = ({ selectedWord }: RootState) =>
    selectedWord.wordIds.length > 0 ||
    (selectedWord.episode && selectedWord.sentenceTimestamp);

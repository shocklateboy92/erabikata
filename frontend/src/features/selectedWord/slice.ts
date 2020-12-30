import { createSelector, createSlice, PayloadAction } from '@reduxjs/toolkit';
import { RootState } from 'app/rootReducer';
import { IWordInfoState } from 'features/wordContext';

interface ISelectedWordState {
    wordBaseForm?: string;
    sentenceTimestamp?: number;
    episode?: string;
}

const getParam = (search: URLSearchParams, param: string): string | undefined =>
    search.get(param) ?? undefined;

const getInitialState = (): ISelectedWordState => {
    const search = new URLSearchParams(window.location.search);
    return {
        wordBaseForm: getParam(search, 'word'),
        sentenceTimestamp: parseInt(search.get('time') ?? ''),
        episode: getParam(search, 'episode')
    };
};

const slice = createSlice({
    initialState: getInitialState(),
    name: 'selectedWord',
    reducers: {
        newWordSelected: (
            state,
            {
                payload: { word, timestamp, episode }
            }: PayloadAction<{
                word: string;
                timestamp?: number;
                episode?: string;
            }>
        ) =>
            state?.wordBaseForm === word &&
            state.sentenceTimestamp === timestamp
                ? // close the panel if the same word is tapped
                  {}
                : {
                      wordBaseForm: word,
                      sentenceTimestamp: timestamp,
                      episode
                  },
        selectionClearRequested: (state) => {}
    }
});

export const selectedWordReducer = slice.reducer;

export const selectSelectedWord = (state: RootState) => state.selectedWord;
export const selectSelectedWordContext = (state: RootState) =>
    (state.selectedWord?.wordBaseForm &&
        state.wordContexts.byId[state.selectedWord.wordBaseForm]) ||
    null;
export const selectSelectedWordInfo = createSelector(
    [selectSelectedWord, selectSelectedWordContext],
    (state: ISelectedWordState | null, context: IWordInfoState) =>
        state?.episode &&
        state.sentenceTimestamp && {
            word: state.wordBaseForm,
            context,
            sentence: { startTime: state.sentenceTimestamp },
            episode: state.episode
        }
);

export const { newWordSelected, selectionClearRequested } = slice.actions;

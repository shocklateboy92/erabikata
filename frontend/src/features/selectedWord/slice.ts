import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { RootState } from 'app/rootReducer';

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
        sentenceTimestamp: parseFloat(search.get('time') ?? ''),
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
        ) => ({
            wordBaseForm: word,
            sentenceTimestamp: timestamp,
            episode
        }),
        selectionClearRequested: (state) => ({}),
        dialogSelection: (state, { payload }: PayloadAction<number>) => ({
            ...state,
            sentenceTimestamp: payload
        })
    }
});

export const selectedWordReducer = slice.reducer;

export const selectIsCurrentlySelected = (
    state: RootState,
    episodeId: string,
    time: number
) =>
    state.selectedWord.episode === episodeId &&
    state.selectedWord.sentenceTimestamp === time;

export const selectSelectedWord = (state: RootState) => state.selectedWord;
export const selectSelectedWordContext = (state: RootState) =>
    (state.selectedWord?.wordBaseForm &&
        state.wordContexts.byId[state.selectedWord.wordBaseForm]) ||
    null;

export const {
    newWordSelected,
    selectionClearRequested,
    dialogSelection
} = slice.actions;

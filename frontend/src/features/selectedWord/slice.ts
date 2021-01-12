import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { RootState } from 'app/rootReducer';
import { selectDialogContent } from 'features/dialog/slice';
import {
    selectEnglishDialogContent,
    selectNearbyEnglishDialog
} from 'features/engDialog/slice';

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
        dialogSelection: (
            state,
            { payload }: PayloadAction<{ time: number; episode?: string }>
        ) => ({
            ...state,
            sentenceTimestamp: payload.time,
            episode: payload.episode ?? state.episode
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
export const selectSelectedDialog = (state: RootState) => {
    const { episode, sentenceTimestamp } = state.selectedWord;
    if (!(episode && sentenceTimestamp)) {
        return;
    }

    // const dialog = selectNearbyDialog(episode, sentenceTimestamp, 1, state);
    // if (dialog.length < 1) {
    //     return;
    // }

    return selectDialogContent(episode, sentenceTimestamp, state);
};
export const selectSelectedEnglishDialog = (state: RootState) => {
    const { episode, sentenceTimestamp } = state.selectedWord;
    if (!episode || !sentenceTimestamp) {
        return;
    }

    const nearest = selectNearbyEnglishDialog(
        state,
        episode,
        sentenceTimestamp,
        1
    );
    if (nearest.length < 1) {
        return;
    }

    return selectEnglishDialogContent(state, episode, nearest[0]);
};

export const {
    newWordSelected,
    selectionClearRequested,
    dialogSelection
} = slice.actions;

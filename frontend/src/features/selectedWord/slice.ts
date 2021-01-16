import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { RootState } from 'app/rootReducer';
import { DialogInfo } from 'backend.generated';
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
        }),

        episodeDialogShift: (
            state,
            {
                payload: { direction, episodeDialog }
            }: PayloadAction<{ direction: -1 | 1; episodeDialog?: number[] }>
        ) => {
            if (!(state.sentenceTimestamp && episodeDialog)) {
                return state;
            }

            const index = episodeDialog.findIndex(
                (d) => d >= state.sentenceTimestamp! // null checked above
            );
            const sentenceTimestamp =
                episodeDialog[
                    Math.max(
                        0,
                        Math.min(episodeDialog.length, index + direction)
                    )
                ];

            return {
                ...state,
                sentenceTimestamp
            };
        },

        dialogWordShift: (
            state,
            {
                payload: { direction, dialog }
            }: PayloadAction<{ dialog?: DialogInfo; direction: 1 | -1 }>
        ) => {
            const word = state.wordBaseForm;
            if (!(dialog && word)) {
                return state;
            }
            let lastIndex: [number, number] = [0, 0];
            for (let lineIdx = 0; lineIdx < dialog.words.length; lineIdx++) {
                const line = dialog.words[lineIdx];
                for (let wordIdx = 0; wordIdx < line.length; wordIdx++) {
                    if (line[wordIdx].baseForm === word) {
                        lastIndex = [lineIdx, wordIdx];
                    }
                }
            }

            let [shiftedRow, shiftedCol] = lastIndex;
            do {
                shiftedCol = shiftedCol + direction;
                if (shiftedCol >= dialog.words[shiftedRow].length) {
                    shiftedRow++;
                    if (shiftedRow >= dialog.words.length) {
                        return state;
                    }
                    shiftedCol = 0;
                }
                if (shiftedCol < 0) {
                    shiftedRow--;
                    if (shiftedRow < 0) {
                        return state;
                    }
                    shiftedCol = dialog.words[shiftedRow].length - 1;
                }
            } while (dialog.words[shiftedRow][shiftedCol].baseForm === ' ');

            const wordBaseForm = dialog.words[shiftedRow][shiftedCol].baseForm;

            return {
                ...state,
                wordBaseForm
            };
        }
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

export const selectSelectedEpisodeContent = (state: RootState) =>
    state.dialog.order[state.selectedWord.episode!];
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
    dialogSelection,
    dialogWordShift,
    episodeDialogShift
} = slice.actions;

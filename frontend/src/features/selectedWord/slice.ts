import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { RootState } from 'app/rootReducer';
import { DialogInfo } from 'backend.generated';
import { selectDialogContent } from 'features/dialog/slice';
import {
    selectEnglishDialogContent,
    selectNearbyEnglishDialog
} from 'features/engDialog/slice';
import { IWordInfoState } from 'features/wordContext';

interface ISelectedWordState {
    wordBaseForm?: string;
    sentenceTimestamp?: number;
    episode?: string;
}

type Direction = 1 | -1;

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
            }: PayloadAction<{ direction: Direction; episodeDialog?: number[] }>
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
            }: PayloadAction<{ dialog?: DialogInfo; direction: Direction }>
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
        },

        occurrenceShift: (
            state,
            {
                payload: { direction, context }
            }: PayloadAction<{ context?: IWordInfoState; direction: Direction }>
        ) => {
            if (!context) {
                return state;
            }

            let index = context.occurrences.findIndex(
                (a) =>
                    a.time === state.sentenceTimestamp &&
                    a.episodeId === state.episode
            );

            if (index < 0) {
                // No occurrence is explicitly selected.
                // Try finding closest one in current episode
                const epOcs = context.occurrences.filter(
                    (o) => o.episodeId === state.episode
                );

                // If the closest occurrence isn't even in this episode,
                // bail out. I don't know what would even cause that.
                if (!epOcs.length) {
                    return state;
                }

                epOcs.sort(
                    (a, b) =>
                        Math.abs(a.time - (state.sentenceTimestamp ?? 0)) -
                        Math.abs(b.time - (state.sentenceTimestamp ?? 0))
                );
                index = context.occurrences.indexOf(epOcs[0]);
            }

            const newIndex = index + direction;
            const newOccurrence =
                context.occurrences[
                    Math.max(0, Math.min(context.occurrences.length, newIndex))
                ];

            return {
                ...state,
                episode: newOccurrence.episodeId,
                sentenceTimestamp: newOccurrence.time
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

export const selectedWordsRelatedToSelectedWord = (state: RootState) => {
    const selectedWord = parseInt(selectSelectedWord(state).wordBaseForm ?? '');
    if (!selectedWord) {
        return;
    }

    const lines = selectSelectedDialog(state)?.words;
    if (!lines) {
        return;
    }

    for (const line of lines)
        for (const word of line) {
            if (word.definitionIds.includes(selectedWord)) {
                return word.definitionIds;
            }
        }
};

export const {
    newWordSelected,
    selectionClearRequested,
    dialogSelection,
    dialogWordShift,
    episodeDialogShift,
    occurrenceShift
} = slice.actions;

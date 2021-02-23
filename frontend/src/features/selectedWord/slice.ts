import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { DialogInfo } from 'backend.generated';
import { selectionClearRequest } from './actions';
import { Entry, WordOccurrence } from '../../backend-rtk.generated';

interface ISelectedWordState {
    sentenceTimestamp?: number;
    episode?: string;
    wordIds: number[];
}

type Direction = 1 | -1;

const getParam = (search: URLSearchParams, param: string): string | undefined =>
    search.get(param) ?? undefined;

const getInitialState = (): ISelectedWordState => {
    const search = new URLSearchParams(window.location.search);
    return {
        sentenceTimestamp: parseFloat(search.get('time') ?? ''),
        episode: getParam(search, 'episode'),
        wordIds: search
            .getAll('word')
            .map((str) => parseInt(str))
            .filter((id) => !isNaN(id))
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
            episode,
            wordIds: []
        }),
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
            }: PayloadAction<{ direction: Direction; episodeDialog?: Entry[] }>
        ) => {
            if (!(state.sentenceTimestamp && episodeDialog)) {
                return state;
            }

            const index = episodeDialog.findIndex(
                (d) => d.time >= state.sentenceTimestamp! // null checked above
            );
            const sentenceTimestamp =
                episodeDialog[
                    Math.max(
                        0,
                        Math.min(episodeDialog.length, index + direction)
                    )
                ].time;

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
            const word = state.wordIds[0];
            if (!(dialog && word)) {
                return state;
            }
            let lastIndex: [number, number] = [0, 0];
            for (let lineIdx = 0; lineIdx < dialog.words.length; lineIdx++) {
                const line = dialog.words[lineIdx];
                for (let wordIdx = 0; wordIdx < line.length; wordIdx++) {
                    if (line[wordIdx].definitionIds[0] === word) {
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
            } while (
                !(dialog.words[shiftedRow][shiftedCol].definitionIds.length > 0)
            );

            const wordIds = dialog.words[shiftedRow][shiftedCol].definitionIds;

            return {
                ...state,
                wordIds
            };
        },

        occurrenceShift: (
            state,
            {
                payload: { direction, context }
            }: PayloadAction<{
                context?: WordOccurrence[];
                direction: Direction;
            }>
        ) => {
            if (!context?.length) {
                return state;
            }

            let index = context.findIndex(
                (a) =>
                    a.time === state.sentenceTimestamp &&
                    a.episodeId === state.episode
            );

            if (index < 0) {
                // No occurrence is explicitly selected.
                // Try finding closest one in current episode
                const epOcs = context.filter(
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
                index = context.indexOf(epOcs[0]);
            }

            const newIndex = index + direction;
            const newOccurrence =
                context[Math.max(0, Math.min(context.length - 1, newIndex))];

            return {
                ...state,
                episode: newOccurrence.episodeId,
                sentenceTimestamp: newOccurrence.time
            };
        },

        selectedWordCycleRequest: ({
            wordIds: [first, ...rest],
            ...state
        }) => ({ ...state, wordIds: first ? [...rest, first] : [] }),

        selectedWordReverseCycleRequest: ({ wordIds, ...state }) => ({
            ...state,
            wordIds:
                wordIds.length > 1
                    ? [
                          wordIds[wordIds.length - 1],
                          ...wordIds.slice(0, wordIds.length - 1)
                      ]
                    : wordIds
        }),

        wordSelectionV2: (state, { payload }: PayloadAction<number[]>) => ({
            ...state,
            wordIds: payload
        }),
        dialogWordSelectionV2: (
            state,
            {
                payload
            }: PayloadAction<{
                wordIds: number[];
                episodeId: string;
                time: number;
                baseForm?: string;
            }>
        ) => ({
            wordIds: payload.wordIds,
            episode: payload.episodeId,
            sentenceTimestamp: payload.time,
            wordBaseForm: payload.baseForm
        })
    },
    extraReducers: (builder) =>
        builder.addCase(selectionClearRequest, () => ({ wordIds: [] }))
});

export const selectedWordReducer = slice.reducer;

export const {
    newWordSelected,
    dialogSelection,
    dialogWordShift,
    episodeDialogShift,
    selectedWordCycleRequest,
    selectedWordReverseCycleRequest,
    occurrenceShift,
    wordSelectionV2,
    dialogWordSelectionV2
} = slice.actions;

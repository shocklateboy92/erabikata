import { createSlice } from '@reduxjs/toolkit';
import { fetchRankedWords } from 'features/rankedWords/slice';
import { RootState } from '../../app/rootReducer';
import { Occurence, WordInfo } from '../../backend.generated';
import { fetchWordIfNeeded } from './api';

export type IWordInfoState =
    | (Omit<WordInfo, 'occurrences'> & {
          occurrences: Omit<Occurence, 'text' | 'subsLink'>[];
      })
    | undefined
    | null;

interface IWordContextState {
    byId: {
        [key: string]: IWordInfoState;
    };
}

const initialState: IWordContextState = {
    byId: {}
};

const slice = createSlice({
    name: 'wordContexts',
    initialState,
    reducers: {},
    extraReducers: (builder) =>
        builder
            .addCase(fetchRankedWords.fulfilled, ({ byId }, { payload }) => ({
                byId: {
                    ...byId,
                    ...Object.fromEntries(
                        payload.map((p) => [p.text, transformWord(p)])
                    )
                }
            }))
            .addCase(fetchWordIfNeeded.fulfilled, ({ byId }, { payload }) => ({
                byId: { ...byId, [payload.text]: transformWord(payload) }
            }))
});

const transformWord = (word: WordInfo) => ({
    ...word,
    occurrences: word.occurrences.map(({ text, subsLink, ...rest }) => rest)
});

export const selectWordInfo = (
    baseForm: string | undefined,
    state: RootState
) => state.wordContexts.byId[baseForm!]; // JS is fine with `undefined` as indexer

export const wordContextsReducer = slice.reducer;

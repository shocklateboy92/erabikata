import { AsyncThunk, createSlice } from '@reduxjs/toolkit';
import { RootState } from 'app/rootReducer';
import { WordInfo, WordsClient } from 'backend.generated';
import { createApiCallThunk } from 'features/auth/api';
import { analyzerChangeRequest } from 'features/backendSelection';

const WORDS_PER_PAGE = 200;

export const fetchRankedWords: AsyncThunk<
    WordInfo[],
    { pageNum: number },
    { state: RootState }
> = createApiCallThunk(
    WordsClient,
    'rankedWords',
    (client, paging, analyzer) => {
        return client.ranked(
            analyzer,
            WORDS_PER_PAGE,
            paging.pageNum * WORDS_PER_PAGE
        );
    },
    {
        condition: ({ pageNum }, { getState }) =>
            !getState().wordRanks.sorted.find(
                (a) => a.rank === pageNum * WORDS_PER_PAGE
            )
    }
);

interface IRankedWordsState {
    sorted: { rank: number; word: string }[];
}
const initialState: IRankedWordsState = { sorted: [] };

const slice = createSlice({
    name: 'wordRanks',
    initialState,
    reducers: {},
    extraReducers: (builder) =>
        builder
            .addCase(fetchRankedWords.fulfilled, (state, { payload }) => ({
                sorted: state.sorted
                    .concat(
                        payload.map((p) => ({ rank: p.rank, word: p.text }))
                    )
                    .sort((a, b) => a.rank - b.rank)
            }))
            // Not going to try keeping different ranks for diferent
            // analyzers. Just ditch old data and fetch new stuff
            .addCase(analyzerChangeRequest, (_) => ({ sorted: [] }))
});

export const wordRanksReducer = slice.reducer;

export const selectRankedWordsArray = (state: RootState, pageNum: number) => {
    const sorted = state.wordRanks.sorted;
    const startIndex = Math.max(
        0,
        Math.min(
            sorted.findIndex((i) => i.rank === pageNum * WORDS_PER_PAGE),
            sorted.length - WORDS_PER_PAGE
        )
    );
    return sorted.slice(startIndex, startIndex + WORDS_PER_PAGE);
};

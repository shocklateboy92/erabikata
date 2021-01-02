import {
    AsyncThunk,
    createAsyncThunk,
    createEntityAdapter,
    createSelector,
    createSlice
} from '@reduxjs/toolkit';
import { RootState } from 'app/rootReducer';
import { PagingInfo, WordInfo, WordsClient } from 'backend.generated';
import {
    analyzerChangeRequest,
    selectAnalyzer,
    selectBaseUrl
} from 'features/backendSelection';

const WORDS_PER_PAGE = 200;

const adapter = createEntityAdapter<{ id: number; text: string }>({
    sortComparer: (a, b) => a.id - b.id
});

export const fetchRankedWords: AsyncThunk<
    WordInfo[],
    { pageNum: number },
    { state: RootState }
> = createAsyncThunk('rankedWords', (paging, { getState }) => {
    const state = getState();
    return new WordsClient(selectBaseUrl(state)).ranked(
        true,
        false,
        WORDS_PER_PAGE,
        paging.pageNum * WORDS_PER_PAGE,
        null,
        selectAnalyzer(state)
    );
});

const slice = createSlice({
    name: 'wordRanks',
    initialState: adapter.getInitialState(),
    reducers: {},
    extraReducers: (builder) =>
        builder
            .addCase(fetchRankedWords.fulfilled, (state, { payload }) =>
                adapter.upsertMany(
                    state,
                    payload.map((p) => ({ id: p.rank, text: p.text }))
                )
            )
            // Not going to try keeping different ranks for diferent
            // analyzers. Just ditch old data and fetch new stuff
            .addCase(analyzerChangeRequest, (state) =>
                adapter.getInitialState()
            )
});

export const wordRanksReducer = slice.reducer;
const selectIds = (
    { ids }: ReturnType<typeof slice.reducer>,
    pageNum: number
) => {
    const start = ids.findIndex((i) => i === pageNum * WORDS_PER_PAGE);
    if (start < 0) {
        return ids.slice(0, WORDS_PER_PAGE);
    }
    if (start + WORDS_PER_PAGE > ids.length) {
        return ids.slice(ids.length - WORDS_PER_PAGE);
    }

    return ids.slice(start, start + WORDS_PER_PAGE);
};

export const selectRankedWords = createSelector(
    [
        (_: RootState, pageNum: number) => pageNum,
        (state) => state.wordRanks,
        (state) => state.wordContexts.byId
    ],
    (pageNum, ranks, contexts) =>
        selectIds(ranks, pageNum).map(
            (id) => contexts[ranks.entities[id]!.text]
        )
);

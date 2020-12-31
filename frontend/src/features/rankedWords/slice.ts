import {
    AsyncThunk,
    createAsyncThunk,
    createEntityAdapter,
    createSlice
} from '@reduxjs/toolkit';
import { RootState } from 'app/rootReducer';
import { PagingInfo, WordInfo, WordsClient } from 'backend.generated';
import {
    analyzerChangeRequest,
    selectAnalyzer,
    selectBaseUrl
} from 'features/backendSelection';

const adapter = createEntityAdapter<{ id: number; text: string }>({
    sortComparer: (a, b) => a.id - b.id
});

export const fetchRankedWords: AsyncThunk<
    WordInfo[],
    PagingInfo,
    { state: RootState }
> = createAsyncThunk('rankedWords', (paging, { getState }) => {
    const params = new URLSearchParams(window.location.search);
    const state = getState();
    return new WordsClient(selectBaseUrl(state)).ranked(
        true,
        !params.get('excludeKnownWords'),
        paging.max,
        paging.skip ?? parseInt(params.get('skip') ?? '0'),
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

export const { selectAll: selectRankedWords } = adapter.getSelectors(
    (state: RootState) => state.wordRanks
);

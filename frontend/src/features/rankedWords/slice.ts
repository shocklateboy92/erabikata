import {
    AsyncThunk,
    createAsyncThunk,
    createEntityAdapter,
    createSlice
} from '@reduxjs/toolkit';
import { RootState } from 'app/rootReducer';
import { PagingInfo, WordInfo, WordsClient } from 'backend.generated';
import { selectBaseUrl } from 'features/backendSelection';

const adapter = createEntityAdapter<{ id: number; text: string }>({
    sortComparer: (a, b) => a.id - b.id
});

export const fetchRankedWords: AsyncThunk<
    WordInfo[],
    PagingInfo,
    { state: RootState }
> = createAsyncThunk('rankedWords', (paging, { getState }) => {
    const params = new URLSearchParams(window.location.search);
    return new WordsClient(selectBaseUrl(getState)).ranked(
        true,
        !params.get('excludeKnownWords'),
        paging.max,
        paging.skip ?? parseInt(params.get('skip') ?? '0')
    );
});

const slice = createSlice({
    name: 'wordRanks',
    initialState: adapter.getInitialState(),
    reducers: {},
    extraReducers: (builder) =>
        builder.addCase(fetchRankedWords.fulfilled, (state, { payload }) =>
            adapter.upsertMany(
                state,
                payload.map((p) => ({ id: p.rank, text: p.text }))
            )
        )
});

export const wordRanksReducer = slice.reducer;

export const { selectAll: selectRankedWords } = adapter.getSelectors(
    (state: RootState) => state.wordRanks
);

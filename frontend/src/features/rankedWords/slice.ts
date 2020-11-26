import {
    createAsyncThunk,
    createEntityAdapter,
    createSlice
} from '@reduxjs/toolkit';
import { RootState } from 'app/rootReducer';
import { PagingInfo, WordsClient } from 'backend.generated';

const adapter = createEntityAdapter<{ id: number; text: string }>({
    sortComparer: (a, b) => a.id - b.id
});

export const fetchRankedWords = createAsyncThunk(
    'rankedWords',
    (paging: PagingInfo, thunkApi) => {
        // const withDefaults = { max: 200, skip: 0, ...paging };
        // const state = thunkApi.getState();

        // if ()

        const params = new URLSearchParams(window.location.search);
        return new WordsClient().ranked(
            true,
            !params.get('excludeKnownWords'),
            paging.max,
            paging.skip ?? parseInt(params.get('skip') ?? '0')
        );
    }
);

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

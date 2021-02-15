import { AsyncThunk, createEntityAdapter, createSlice } from '@reduxjs/toolkit';
import { RootState } from 'app/rootReducer';
import { WordClient, WordDefinition } from 'backend.generated';
import { createApiCallThunk } from 'features/auth/api';

export interface IJishoWord {
    isCommon: boolean;
    slug: string;
    japanese: { word: string; reading: string }[];
    english: {
        tags: string[];
        senses: string[];
    }[];
}

export interface IWordDefinition {
    baseForm: string;
    exact: WordDefinition[];
    related: WordDefinition[];
}

const adapter = createEntityAdapter<WordDefinition>({
    // selectId: (word) => word.id
});
const thunk: AsyncThunk<WordDefinition, string, {}> = createApiCallThunk(
    WordClient,
    'wordDefinitions',
    (client, word) => client.definition(word),
    {
        condition: (baseForm, { getState }) => {
            return !selectDefinitionById(getState(), baseForm);
        }
    }
);

const slice = createSlice({
    name: 'wordDefinitions',
    initialState: adapter.getInitialState(),
    reducers: {},
    extraReducers: (builder) =>
        builder.addCase(
            thunk.fulfilled,
            (state, { payload, meta: { arg: baseForm } }) => {
                adapter.upsertOne(state, payload);
            }
        )
});

export const {
    selectById: selectDefinitionById
} = adapter.getSelectors<RootState>((state) => state.wordDefinitions);

export const fetchDefinitionsIfNeeded = thunk;

export const wordDefinitionReducer = slice.reducer;

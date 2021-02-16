import { AsyncThunk, createSlice } from '@reduxjs/toolkit';
import { RootState } from 'app/rootReducer';
import { WordDefinition, WordsClient } from 'backend.generated';
import { createApiCallThunk } from 'features/auth/api';
import { selectSelectedWords } from 'features/selectedWord';

export interface IWordDefinition {
    baseForm: string;
    exact: WordDefinition[];
    related: WordDefinition[];
}

const thunk: AsyncThunk<WordDefinition[], number[], {}> = createApiCallThunk(
    WordsClient,
    'fetchDefinitions',
    (client, words) => client.definition(words),
    {
        condition: (wordIds, { getState }) => {
            return (
                selectDefinitionsById(getState(), wordIds).length !==
                wordIds.length
            );
        }
    }
);

interface IWordDefinitionState {
    byId: { [key: number]: WordDefinition | undefined };
}

const initialState: IWordDefinitionState = { byId: {} };

const slice = createSlice({
    name: 'wordDefinitions',
    initialState,
    reducers: {},
    extraReducers: (builder) =>
        builder.addCase(
            thunk.fulfilled,
            (state, { payload, meta: { arg: baseForm } }) => ({
                byId: {
                    ...state.byId,
                    ...Object.fromEntries(payload.map((e) => [e.id, e]))
                }
            })
        )
});

export const selectDefinitionsById = (state: RootState, wordIds: number[]) =>
    wordIds
        .map((id) => state.wordDefinitions.byId[id])
        .filter((def): def is WordDefinition => def !== undefined);

export const selectSelectedWordDefinitions = (state: RootState) =>
    selectDefinitionsById(state, selectSelectedWords(state));

export const fetchDefinitionsIfNeeded = thunk;

export const wordDefinitionReducer = slice.reducer;

import { AsyncThunk, createSlice } from '@reduxjs/toolkit';
import { RootState } from 'app/rootReducer';
import { WordDefinition, WordsClient } from 'backend.generated';
import { createApiCallThunk } from 'features/auth/api';
import { selectSelectedWords } from 'features/selectedWord';

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
    readingsOnly: boolean;
}

const initialState: IWordDefinitionState = {
    byId: {},
    readingsOnly: false
};

const slice = createSlice({
    name: 'wordDefinitions',
    initialState,
    reducers: {
        readingsOnlyModeToggle: (state) => ({
            ...state,
            readingsOnly: !state.readingsOnly
        })
    },
    extraReducers: (builder) =>
        builder.addCase(thunk.fulfilled, (state, { payload }) => ({
            ...state,
            byId: {
                ...state.byId,
                ...Object.fromEntries(payload.map((e) => [e.id, e]))
            }
        }))
});

export const selectDefinitionsById = (state: RootState, wordIds: number[]) =>
    wordIds
        .map((id) => state.wordDefinitions.byId[id])
        .filter((def): def is WordDefinition => def !== undefined);

export const selectSelectedWordDefinitions = (state: RootState) =>
    selectDefinitionsById(state, selectSelectedWords(state));

export const fetchDefinitionsIfNeeded = thunk;

export const { readingsOnlyModeToggle } = slice.actions;
export const wordDefinitionReducer = slice.reducer;

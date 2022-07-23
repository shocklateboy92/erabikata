import { createSlice } from '@reduxjs/toolkit';
import { RootState } from 'app/rootReducer';
import { apiEndpoints } from 'backend';
import { WordDefinition } from 'backend.generated';
import { selectSelectedWords } from 'features/selectedWord';

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
        builder.addMatcher(
            apiEndpoints.wordsDefinition.matchFulfilled,
            (state, { payload }) => ({
                ...state,
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

export const fetchDefinitionsIfNeeded = apiEndpoints.wordsDefinition.initiate;

export const { readingsOnlyModeToggle } = slice.actions;
export const wordDefinitionReducer = slice.reducer;

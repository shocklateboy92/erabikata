import { AsyncThunk, createSlice } from '@reduxjs/toolkit';
import { RootState } from 'app/rootReducer';
import { WordDefinition, WordRank, WordsClient } from 'backend.generated';
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

export const fetchEpisodeRanksIfNeeded: AsyncThunk<
    WordRank[],
    [episodeId: number, wordIds: number[]],
    {}
> = createApiCallThunk(
    WordsClient,
    'fetchEpisodeRanks',
    (client, args, analyzer) => client.episodeRank(analyzer, ...args),
    {
        condition: ([episodeId, wordIds], { getState }) =>
            selectEpisodeRanks(getState(), episodeId, wordIds).length !==
            wordIds.length
    }
);

interface IWordDefinitionState {
    byId: { [key: number]: WordDefinition | undefined };
    episodeRanks: { [key: number]: { [key: number]: number | undefined } };
}

const initialState: IWordDefinitionState = { byId: {}, episodeRanks: {} };

const slice = createSlice({
    name: 'wordDefinitions',
    initialState,
    reducers: {},
    extraReducers: (builder) =>
        builder.addCase(
            thunk.fulfilled,
            (state, { payload, meta: { arg: baseForm } }) => ({
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

export const selectEpisodeRanks = (
    state: RootState,
    episodeId: number,
    wordIds: number[]
) => {
    const episode = state.wordDefinitions.episodeRanks[episodeId] ?? {};
    return wordIds.map((id) => episode[id]);
};

export const selectSelectedWordDefinitions = (state: RootState) =>
    selectDefinitionsById(state, selectSelectedWords(state));

export const fetchDefinitionsIfNeeded = thunk;

export const wordDefinitionReducer = slice.reducer;

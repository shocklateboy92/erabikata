import { AsyncThunk, createSlice } from '@reduxjs/toolkit';
import { RootState } from 'app/rootReducer';
import { WordDefinition, WordRank, WordsClient } from 'backend.generated';
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

export const fetchEpisodeRanksIfNeeded: AsyncThunk<
    WordRank[],
    [episodeId: string, wordIds: number[]],
    {}
> = createApiCallThunk(
    WordsClient,
    'fetchEpisodeRanks',
    (client, args, analyzer) => client.episodeRank(analyzer, ...args),
    {
        condition: ([episodeId, wordIds], { getState }) => {
            const episode = getState().wordDefinitions.episodeRanks[episodeId];
            return !episode || !!wordIds.find((word) => !episode[word]);
        }
    }
);

interface IWordDefinitionState {
    byId: { [key: number]: WordDefinition | undefined };
    episodeRanks: { [key: string]: { [key: number]: number | undefined } };
    readingsOnly: boolean;
}

const initialState: IWordDefinitionState = {
    byId: {},
    episodeRanks: {},
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
        builder
            .addCase(
                thunk.fulfilled,
                (state, { payload, meta: { arg: baseForm } }) => ({
                    ...state,
                    byId: {
                        ...state.byId,
                        ...Object.fromEntries(payload.map((e) => [e.id, e]))
                    }
                })
            )
            .addCase(
                fetchEpisodeRanksIfNeeded.fulfilled,
                (
                    state,
                    {
                        payload,
                        meta: {
                            arg: [episodeId]
                        }
                    }
                ) => ({
                    ...state,
                    episodeRanks: {
                        ...state.episodeRanks,
                        [episodeId]: {
                            ...state.episodeRanks[episodeId],
                            ...Object.fromEntries(
                                payload.map((rank) => [rank.id, rank.rank])
                            )
                        }
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

export const { readingsOnlyModeToggle } = slice.actions;
export const wordDefinitionReducer = slice.reducer;

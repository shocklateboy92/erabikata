import { RootState } from 'app/rootReducer';

export const selectWordDefinition = (state: RootState, wordId: number) =>
    state.wordDefinitions.byId[wordId];

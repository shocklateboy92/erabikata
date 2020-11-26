import { createSelector, createSlice, PayloadAction } from '@reduxjs/toolkit';
import { RootState } from 'app/rootReducer';
import {
    IWordInfoState,
    wordContextFetchSucceeded
} from 'features/wordContext';

interface ISelectedWordState {
    wordBaseForm: string;
    sentenceTimestamp?: number;
    episode?: string;
}

const slice = createSlice({
    initialState: null as ISelectedWordState | null,
    name: 'selectedWord',
    reducers: {
        newWordSelected: (
            state,
            {
                payload: { word, timestamp, episode }
            }: PayloadAction<{
                word: string;
                timestamp?: number;
                episode?: string;
            }>
        ) =>
            state?.wordBaseForm === word &&
            state.sentenceTimestamp === timestamp
                ? // close the panel if the same word is tapped
                  null
                : {
                      wordBaseForm: word,
                      sentenceTimestamp: timestamp,
                      episode
                  },
        selectionClearRequested: (state) => null
    },
    extraReducers: (builder) =>
        builder.addCase(
            wordContextFetchSucceeded,
            (
                state,
                {
                    payload: {
                        occurrences: [occurence],
                        text
                    }
                }
            ) => {
                if (occurence) {
                    // If this is a full fetch, we probably just navigated to a word page
                    // Make sure that word is selected, so we can easily see the definiton.
                    return {
                        episode: occurence.episodeId,
                        sentenceTimestamp: occurence.time,
                        wordBaseForm: text
                    };
                }
                return state;
            }
        )
});

export const selectedWordReducer = slice.reducer;

export const selectSelectedWord = (state: RootState) => state.selectedWord;
export const selectSelectedWordInfo = createSelector(
    [
        selectSelectedWord,
        (state: RootState) =>
            (state.selectedWord?.wordBaseForm &&
                state.wordContexts.byId[state.selectedWord.wordBaseForm]) ||
            null
    ],
    (state: ISelectedWordState | null, context: IWordInfoState) =>
        state?.episode &&
        state.sentenceTimestamp && {
            word: state.wordBaseForm,
            context,
            sentence: { startTime: state.sentenceTimestamp },
            episode: state.episode
        }
);

export const { newWordSelected, selectionClearRequested } = slice.actions;

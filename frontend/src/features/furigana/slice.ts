import { createAsyncThunk, createSlice } from '@reduxjs/toolkit';
import { RootState } from 'app/rootReducer';
import { AppDispatch } from 'app/store';
import { apiEndpoints } from 'backend';

const initialState: {
    enabled: boolean;
    words: {
        [wordId: number]: { hide: boolean } | undefined;
    };
} = {
    enabled: true,
    words: {}
};

const slice = createSlice({
    name: 'furigana',
    initialState,
    reducers: {
        toggleFurigana: (state) => ({ ...state, enabled: !state.enabled })
    },
    extraReducers: (builder) =>
        builder.addMatcher(
            apiEndpoints.wordsWithReadingsKnown.matchFulfilled,
            (state, { payload: { result } }) => ({
                ...state,
                words: Object.fromEntries(
                    result.map((id) => [id, { hide: true }])
                )
            })
        )
});

export const toggleWordFurigana = createAsyncThunk<
    void,
    number,
    { state: RootState; dispatch: AppDispatch }
>('toggleWordFurigana', async (wordId, { getState, dispatch }) => {
    const isHidden = selectIsFuriganaHiddenForWords(getState(), [wordId]);
    await dispatch(
        apiEndpoints.executeAction.initiate(
            {
                activityType: isHidden ? 'UnLearnReading' : 'LearnReading',
                wordId
            },
            { track: false }
        )
    );

    await dispatch(fetchWordsWithHiddenFurigana());
});

export const fetchWordsWithHiddenFurigana = () =>
    apiEndpoints.wordsWithReadingsKnown.initiate({}, { subscribe: false });

export const selectIsFuriganaEnabled = (state: RootState) =>
    state.furigana.enabled;
export const selectIsFuriganaHiddenForWords = (
    state: RootState,
    wordIds: number[]
) => wordIds.find((id) => state.furigana.words[id]?.hide);

export const { toggleFurigana } = slice.actions;

export const furiganaReducer = slice.reducer;

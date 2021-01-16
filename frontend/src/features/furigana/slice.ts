import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { RootState } from 'app/rootReducer';

const initialState: {
    enabled: boolean;
    words: {
        [wordBaseForm: string]: { hide: boolean } | undefined;
    };
} = {
    enabled: true,
    words: {}
};

const slice = createSlice({
    name: 'furigana',
    initialState,
    reducers: {
        toggleFurigana: (state) => ({ ...state, enabled: !state.enabled }),
        toggleWordFurigana: (state, { payload }: PayloadAction<string>) => ({
            ...state,
            words: {
                ...state.words,
                [payload]: {
                    hide: !state.words[payload]?.hide
                }
            }
        })
    }
});

export const selectIsFuriganaEnabled = (state: RootState) =>
    state.furigana.enabled;
export const selectIsFuriganaHiddenForWord = (
    state: RootState,
    baseForm: string
) => state.furigana.words[baseForm]?.hide;

export const { toggleFurigana, toggleWordFurigana } = slice.actions;

export const furiganaReducer = slice.reducer;

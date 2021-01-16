import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { RootState } from 'app/rootReducer';

const initialState: {
    enabled: boolean;
    words: {
        [wordBaseForm: string]: { localEnable: boolean } | undefined;
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
                    localEnable: !state.words[payload]?.localEnable
                }
            }
        })
    }
});

export const selectIsFuriganaEnabled = (state: RootState) =>
    state.furigana.enabled;

export const { toggleFurigana } = slice.actions;

export const furiganaReducer = slice.reducer;

import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { RootState } from 'app/rootReducer';

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
        toggleFurigana: (state) => ({ ...state, enabled: !state.enabled }),
        toggleWordFurigana: (state, { payload }: PayloadAction<number[]>) => ({
            ...state,
            words: {
                ...state.words,
                ...Object.fromEntries(
                    payload.map((id) => [id, { hide: !state.words[id]?.hide }])
                )
            }
        })
    }
});

export const selectIsFuriganaEnabled = (state: RootState) =>
    state.furigana.enabled;
export const selectIsFuriganaHiddenForWords = (
    state: RootState,
    wordIds: number[]
) => wordIds.find((id) => state.furigana.words[id]?.hide);

export const { toggleFurigana, toggleWordFurigana } = slice.actions;

export const furiganaReducer = slice.reducer;

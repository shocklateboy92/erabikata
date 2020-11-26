import { createSlice } from '@reduxjs/toolkit';
import { RootState } from 'app/rootReducer';

const initialState = {
    enabled: true
};

const slice = createSlice({
    name: 'furigana',
    initialState,
    reducers: {
        toggleFurigana: (state) => ({ enabled: !state.enabled })
    }
});

export const selectIsFuriganaEnabled = (state: RootState) =>
    state.furigana.enabled;

export const { toggleFurigana } = slice.actions;

export const furiganaReducer = slice.reducer;

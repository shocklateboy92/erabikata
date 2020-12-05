import { createSlice } from '@reduxjs/toolkit';

const initialState: {
    enabled: boolean;
} = { enabled: false };

const slice = createSlice({
    name: 'wakeLock',
    initialState,
    reducers: {
        enabledToggle: (state) => ({ enabled: !state.enabled })
    }
});

export const { enabledToggle } = slice.actions;
export const wakeLockReducer = slice.reducer;

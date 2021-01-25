import { createSlice } from '@reduxjs/toolkit';
import { initializeTodoist } from './api';

interface ITodoistState {
    authToken?: string;
}
const initialState: ITodoistState = {};

const slice = createSlice({
    name: 'todoist',
    initialState,
    reducers: {},
    extraReducers: (builder) =>
        builder.addCase(initializeTodoist.fulfilled, (state, { payload }) => ({
            authToken: payload
        }))
});

export const todoistReducer = slice.reducer;

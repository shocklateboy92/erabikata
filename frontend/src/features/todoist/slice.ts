import { createSlice } from '@reduxjs/toolkit';
import { fetchCandidateTasks, initializeTodoist } from './api';

interface ITodoistState {
    authToken?: string;
    candidateTasks: {
        id: number;
        content: string;
    }[];
    selectedTask: number;
}
const initialState: ITodoistState = {
    candidateTasks: [],
    selectedTask: -1
};

const slice = createSlice({
    name: 'todoist',
    initialState,
    reducers: {},
    extraReducers: (builder) =>
        builder
            .addCase(initializeTodoist.fulfilled, (state, { payload }) => ({
                ...state,
                authToken: payload
            }))
            .addCase(fetchCandidateTasks.fulfilled, (state, { payload }) => ({
                ...state,
                candidateTasks: payload.map(({ id, content }) => ({
                    id,
                    content
                })),
                selectedTask: payload.findIndex()
            }))
});

export const todoistReducer = slice.reducer;

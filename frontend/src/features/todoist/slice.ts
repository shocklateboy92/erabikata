import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { fetchCandidateTasks, initializeTodoist } from './api';

interface ITodoistState {
    authToken?: string;
    candidateTasks: {
        id: number;
        content: string;
    }[];
    userSelectedTaskId?: number;
}
const initialState: ITodoistState = {
    candidateTasks: []
};

const slice = createSlice({
    name: 'todoist',
    initialState,
    reducers: {
        userTaskSelection: (state, { payload }: PayloadAction<number>) => ({
            ...state,
            userSelectedTask: payload
        })
    },
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
                }))
            }))
});

export const todoistReducer = slice.reducer;

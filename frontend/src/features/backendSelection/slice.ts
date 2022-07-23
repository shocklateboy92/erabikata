import { createSlice } from '@reduxjs/toolkit';
import { RootState } from 'app/rootReducer';
import { getInitialBaseUrl } from './api';

interface IBackendSelection {
    baseUrl: string;
}

const initialState: IBackendSelection = {
    baseUrl: getInitialBaseUrl()
};

const slice = createSlice({
    name: 'backendSelection',
    initialState,
    reducers: {}
});

export const selectBaseUrl = (state: RootState | (() => RootState)) => {
    if (typeof state === 'function') {
        state = state();
    }

    return state.backend.baseUrl;
};

export const backendReducer = slice.reducer;

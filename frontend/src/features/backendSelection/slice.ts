import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { RootState } from 'app/rootReducer';
import { Analyzer } from 'backend.generated';
import { getInitialBaseUrl } from './api';

interface IBackendSelection {
    baseUrl: string;
    analyzer: Analyzer;
}

const initialState: IBackendSelection = {
    analyzer: 'SudachiC',
    baseUrl: getInitialBaseUrl()
};

const slice = createSlice({
    name: 'backendSelection',
    initialState,
    reducers: {
        analyzerChangeRequest: (
            state,
            { payload }: PayloadAction<Analyzer>
        ) => ({ ...state, analyzer: payload })
    }
});

export const selectAnalyzer = (state: RootState) => state.backend.analyzer;
export const selectBaseUrl = (state: RootState | (() => RootState)) => {
    if (typeof state === 'function') {
        state = state();
    }

    return state.backend.baseUrl;
};

export const { analyzerChangeRequest } = slice.actions;

export const backendReducer = slice.reducer;

import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { RootState } from 'app/rootReducer';
import { Analyzer } from 'backend.generated';

interface IBackendSelection {
    baseUrl: string;
    analyzer: Analyzer;
}

const params = new URLSearchParams(window.location.search);
const initialState: IBackendSelection = {
    analyzer: 'Kuromoji',
    baseUrl:
        params.get('env') ??
        // Convenience override for frontend-only local dev
        window.location.origin === 'http://localhost:3000'
            ? 'https://erabikata2.apps.lasath.org'
            : window.location.origin
};

const slice = createSlice({
    name: 'backendSelection',
    initialState,
    reducers: {
        analyzerChangeRequest: (
            state,
            { payload }: PayloadAction<Analyzer>
        ) => ({
            ...state,
            analyzer: payload
        })
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

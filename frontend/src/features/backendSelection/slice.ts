import { createSlice } from '@reduxjs/toolkit';
import { RootState } from 'app/rootReducer';

interface IBackendSelection {
    baseUrl: string;
}

const params = new URLSearchParams(window.location.search);
const initialState: IBackendSelection = {
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
    reducers: {}
});

export const selectBaseUrl = (state: RootState | (() => RootState)) => {
    if (typeof state === 'function') {
        state = state();
    }

    return state.backend.baseUrl;
};
export const backendReducer = slice.reducer;

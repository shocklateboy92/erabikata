import { createSlice } from '@reduxjs/toolkit';
import { RootState } from 'app/rootReducer';
import './api';
import { getIsUserSignedIn } from './api';

interface IAuthState {
    isSignedIn: boolean;
}

const initialState: IAuthState = {
    isSignedIn: getIsUserSignedIn()
};

const slice = createSlice({
    name: 'auth',
    initialState,
    reducers: {
        authenticationError: (state) => ({ isSignedIn: false }),
        authenticationSuccess: (state) => ({ isSignedIn: true })
    }
    // NOTE: There are no extra reducers from './api.ts' because
    // that file depends on this, creating a circular reference.
});

export const authReducer = slice.reducer;

export const selectIsUserSignedIn = (state: RootState) => state.auth.isSignedIn;

export const { authenticationError, authenticationSuccess } = slice.actions;

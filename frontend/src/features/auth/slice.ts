import { createSlice } from '@reduxjs/toolkit';
import { RootState } from 'app/rootReducer';
import './api';
import { getIsUserSignedIn } from './api';

interface IAuthState {
    isSignedIn: boolean;
    todoistToken?: string;
}

const initialState: IAuthState = {
    isSignedIn: getIsUserSignedIn()
};

const slice = createSlice({
    name: 'auth',
    initialState,
    reducers: {
        authenticationError: (state) => ({ isSignedIn: false })
    }
});

export const authReducer = slice.reducer;

export const selectIsUserSignedIn = (state: RootState) => state.auth.isSignedIn;

export const { authenticationError } = slice.actions;

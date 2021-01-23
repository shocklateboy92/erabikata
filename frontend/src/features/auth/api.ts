import { AsyncThunk, createAsyncThunk } from '@reduxjs/toolkit';
import { RootState } from 'app/rootReducer';
import { selectBaseUrl } from 'features/backendSelection';
import { UserManager } from 'oidc-client';
import React, { useContext } from 'react';

interface IAuthContext {
    userManager?: UserManager;
}

type AppAsyncThunk<ReturnType, ArgumentType> = AsyncThunk<
    ReturnType,
    IAuthContext,
    { state: RootState }
>;

const createContext = (state: RootState | (() => RootState)) => {
    const baseUrl = selectBaseUrl(state);
    return new UserManager({
        authority: baseUrl,
        client_id: 'frontend',
        redirect_uri: window.location.href
    });
};

export const signIn: AppAsyncThunk<void, void> = createAsyncThunk(
    'signIn',
    async (context, { getState }) => {
        context.userManager = context.userManager ?? createContext(getState);

        await context.userManager.signinRedirect();
    }
);

const AuthContext = React.createContext<IAuthContext>({});
export const useAuth = () => useContext(AuthContext);

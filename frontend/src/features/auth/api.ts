import { PublicClientApplication } from '@azure/msal-browser';
import { AsyncThunk, createAsyncThunk } from '@reduxjs/toolkit';
import { RootState } from 'app/rootReducer';
import React, { useContext } from 'react';

interface IAuthContext {
    userManager?: PublicClientApplication;
}

type AuthAsyncThunk<ReturnType> = AsyncThunk<
    ReturnType,
    IAuthContext,
    { state: RootState }
>;

const createContext = () => {
    return new PublicClientApplication({
        auth: {
            clientId: 'cb9157f3-50fe-47bf-af0b-77c976a2a698',
            redirectUri: window.location.origin + '/settings',
            authority: 'https://login.microsoftonline.com/consumers'
        }
    });
};

export const signIn: AuthAsyncThunk<void> = createAsyncThunk(
    'signIn',
    async (context) => {
        context.userManager = context.userManager ?? createContext();

        console.log('startgin');
        const token = await context.userManager.loginRedirect({
            scopes: ['User.Read']
        });
        console.log('finishe');
        console.log(token);
    }
);

export const checkSignIn: AuthAsyncThunk<void> = createAsyncThunk(
    'checkSignInResult',
    async (context) => {
        context.userManager = context.userManager ?? createContext();
        console.log(context.userManager.getAllAccounts());
        const accounts = await context.userManager.handleRedirectPromise();
        console.log(accounts);
    }
);

const AuthContext = React.createContext<IAuthContext>({});
export const useAuth = () => useContext(AuthContext);

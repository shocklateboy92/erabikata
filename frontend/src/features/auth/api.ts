import { PublicClientApplication } from '@azure/msal-browser';
import { createAsyncThunk } from '@reduxjs/toolkit';
import store from 'app/store';
import { authenticationSuccess } from './slice';

const scopes = ['api://cb9157f3-50fe-47bf-af0b-77c976a2a698/Data.Store'];
const userManager = new PublicClientApplication({
    auth: {
        clientId: 'cb9157f3-50fe-47bf-af0b-77c976a2a698',
        redirectUri: window.location.origin + '/ui/settings',
        authority: 'https://login.microsoftonline.com/consumers'
    }
});

userManager
    .handleRedirectPromise()
    .then((token) => token && store.dispatch(authenticationSuccess()));

export const signIn = createAsyncThunk('signIn', () => {
    userManager.loginRedirect({
        scopes
    });
});

export const getIsUserSignedIn = () => userManager.getAllAccounts().length > 0;

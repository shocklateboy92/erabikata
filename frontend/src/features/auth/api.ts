import { PublicClientApplication } from '@azure/msal-browser';
import { createAsyncThunk } from '@reduxjs/toolkit';
import { RootState } from 'app/rootReducer';
import { Analyzer, IApiClientConfig } from 'backend.generated';
import { selectAnalyzer, selectBaseUrl } from 'features/backendSelection';
import { notification } from 'features/notifications';
import { authenticationError, selectIsUserSignedIn } from './slice';

const scopes = ['api://cb9157f3-50fe-47bf-af0b-77c976a2a698/Data.Store'];
const userManager = new PublicClientApplication({
    auth: {
        clientId: 'cb9157f3-50fe-47bf-af0b-77c976a2a698',
        redirectUri: window.location.origin + '/settings',
        authority: 'https://login.microsoftonline.com/consumers'
    }
});

userManager.handleRedirectPromise();

export const signIn = createAsyncThunk('signIn', () => {
    userManager.loginRedirect({
        scopes
    });
});

export const getIsUserSignedIn = () => userManager.getAllAccounts().length > 0;

export function createApiCallThunk<
    ClientType,
    ReturnType = unknown,
    ArgumentType = void
>(
    constructor: {
        new (config: IApiClientConfig, baseUrl: string): ClientType;
    },
    name: string,
    payloadCreator: (
        client: ClientType,
        args: ArgumentType,
        analyzer: Analyzer
    ) => Promise<ReturnType>,
    options?: {
        condition: (
            arg: ArgumentType,
            thunk: { getState: () => RootState }
        ) => boolean;
    }
) {
    return createAsyncThunk<ReturnType, ArgumentType, { state: RootState }>(
        name,
        async (arg, thunk) => {
            const state = thunk.getState();
            const config: IApiClientConfig = {};
            if (selectIsUserSignedIn(state)) {
                try {
                    config.authToken = (
                        await userManager.acquireTokenSilent({
                            scopes,
                            account: userManager.getAllAccounts()[0]
                        })
                    ).accessToken;
                } catch (exception) {
                    console.log(exception);
                    thunk.dispatch(authenticationError());
                    thunk.dispatch(
                        notification({
                            title: 'Sign in again',
                            text:
                                'Something went wrong trying to access a protected resource. Signing in again might fix the problem.'
                        })
                    );
                }
            }
            return payloadCreator(
                new constructor(config, selectBaseUrl(state)),
                arg,
                selectAnalyzer(state)
            );
        },
        options
    );
}

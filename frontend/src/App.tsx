import { AppInsightsContext } from '@microsoft/applicationinsights-react-js';
import appHistory from 'appHistory';
import { reactPlugin } from 'appInsights';
import React from 'react';
import { Provider } from 'react-redux';
import { unstable_HistoryRouter as HistoryRouter } from 'react-router-dom';
import store from './app/store';
import { AppSwitch } from './features/routing/switch';

export const App = () => (
    <React.StrictMode>
        <Provider store={store}>
            <AppInsightsContext.Provider value={reactPlugin}>
                <HistoryRouter history={appHistory}>
                    <AppSwitch />
                </HistoryRouter>
            </AppInsightsContext.Provider>
        </Provider>
    </React.StrictMode>
);

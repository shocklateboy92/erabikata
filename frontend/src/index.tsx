import { AppInsightsContext } from '@microsoft/applicationinsights-react-js';
import appHistory from 'appHistory';
import { reactPlugin } from 'appInsights';
import React from 'react';
import ReactDOM from 'react-dom';
import { Provider } from 'react-redux';
import { unstable_HistoryRouter as HistoryRouter } from 'react-router-dom';
import './App.css';
import './app.scss';
import store from './app/store';
import { AppSwitch } from './features/routing/switch';
import './index.css';
import * as serviceWorker from './serviceWorkerRegistration';

ReactDOM.render(
    <React.StrictMode>
        <Provider store={store}>
            <AppInsightsContext.Provider value={reactPlugin}>
                <HistoryRouter history={appHistory}>
                    <AppSwitch />
                </HistoryRouter>
            </AppInsightsContext.Provider>
        </Provider>
    </React.StrictMode>,
    document.getElementById('root')
);

// If you want your app to work offline and load faster, you can change
// unregister() to register() below. Note this comes with some pitfalls.
// Learn more about service workers: https://bit.ly/CRA-PWA
serviceWorker.register();

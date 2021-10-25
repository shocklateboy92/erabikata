import React from 'react';
import ReactDOM from 'react-dom';
import './index.css';
import './App.css';
import './app.scss';
import * as serviceWorker from './serviceWorkerRegistration';
import { Provider } from 'react-redux';
import store from './app/store';
import { Router } from 'react-router-dom';
import { AppSwitch } from './features/routing/switch';
import history from './appHistory';
import { AppInsightsContext } from '@microsoft/applicationinsights-react-js';
import { reactPlugin } from 'appInsights';

ReactDOM.render(
    <React.StrictMode>
        <Provider store={store}>
            <AppInsightsContext.Provider value={reactPlugin}>
                <Router history={history}>
                    <AppSwitch />
                </Router>
            </AppInsightsContext.Provider>
        </Provider>
    </React.StrictMode>,
    document.getElementById('root')
);

// If you want your app to work offline and load faster, you can change
// unregister() to register() below. Note this comes with some pitfalls.
// Learn more about service workers: https://bit.ly/CRA-PWA
serviceWorker.register();

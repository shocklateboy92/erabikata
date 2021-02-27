import React from 'react';
import ReactDOM from 'react-dom';
import './index.css';
import './App.css';
import './app.scss';
import * as serviceWorker from './serviceWorkerRegistration';
import { Provider } from 'react-redux';
import store from './app/store';
import { HassContext } from 'features/hass';
import { BrowserRouter } from 'react-router-dom';
import { AppSwitch } from './features/routing/switch';

ReactDOM.render(
    <React.StrictMode>
        <HassContext.Provider value={{}}>
            <Provider store={store}>
                <BrowserRouter>
                    <AppSwitch />
                </BrowserRouter>
            </Provider>
        </HassContext.Provider>
    </React.StrictMode>,
    document.getElementById('root')
);

// If you want your app to work offline and load faster, you can change
// unregister() to register() below. Note this comes with some pitfalls.
// Learn more about service workers: https://bit.ly/CRA-PWA
serviceWorker.register();

import React from 'react';
import { BrowserRouter } from 'react-router-dom';
import './App.css';
import './app.scss';
import { AppSwitch } from './features/routing/switch';

function App() {
    return (
        <BrowserRouter>
            <AppSwitch />
        </BrowserRouter>
    );
}

export default App;

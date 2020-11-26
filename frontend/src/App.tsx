import { LoginPage } from 'features/login';
import { DialogPage } from 'pages/dialogPage';
import { InfoPage } from 'pages/infoPage';
import { RankedWordsPage } from 'pages/rankedWordsPage';
import { SearchWordPage, WordPage } from 'pages/wordPage';
import React from 'react';
import { BrowserRouter, Redirect, Route, Switch } from 'react-router-dom';
import './App.css';
import './app.scss';
import { NowPlaying } from './features/nowPlaying';
import { StatusMessages } from './features/statusMessages';

function App() {
    return (
        <BrowserRouter>
            <Switch>
                <Route path="/login">
                    <LoginPage />
                </Route>
                <Route path="/word/:dictionaryForm">
                    <WordPage />
                </Route>
                <Route path="/word">
                    <SearchWordPage />
                </Route>
                <Route path="/dialog">
                    <DialogPage />
                </Route>
                <Route path="/rankedWords">
                    <RankedWordsPage />
                </Route>
                <Route path="/settings">
                    <InfoPage />
                </Route>
                <Route path="/nowPlaying">
                    <StatusMessages />
                    <NowPlaying />
                </Route>
                <Route>
                    <Redirect to="/nowPlaying" />
                </Route>
            </Switch>
        </BrowserRouter>
    );
}

export default App;

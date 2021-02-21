import { DialogPage } from 'pages/dialogPage';
import { InfoPage } from 'pages/infoPage';
import { RankedWordsPage } from 'pages/rankedWordsPage';
import { SearchWordPage, WordPage } from 'pages/wordPage';
import React from 'react';
import { BrowserRouter, Redirect, Route, Switch } from 'react-router-dom';
import './App.css';
import './app.scss';
import { NowPlaying } from './features/nowPlaying';

function App() {
    return (
        <BrowserRouter>
            <Switch>
                <Route path="/ui/word/:dictionaryForm">
                    <WordPage />
                </Route>
                <Route path="/ui/word">
                    <SearchWordPage />
                </Route>
                <Route path="/ui/dialog">
                    <DialogPage />
                </Route>
                <Route path="/ui/rankedWords/:pageNum?">
                    <RankedWordsPage />
                </Route>
                <Route path="/ui/settings">
                    <InfoPage />
                </Route>
                <Route path="/ui/nowPlaying">
                    <NowPlaying />
                </Route>
                <Route>
                    <Redirect to="/ui/nowPlaying" />
                </Route>
            </Switch>
        </BrowserRouter>
    );
}

export default App;

import { DialogPage } from 'pages/dialogPage';
import { InfoPage } from 'pages/infoPage';
import { RankedWordsPage } from 'pages/rankedWordsPage';
import { SearchWordPage, WordPage } from 'pages/wordPage';
import React, { FC } from 'react';
import { BrowserRouter, Redirect, Route, Switch } from 'react-router-dom';
import './App.css';
import './app.scss';
import { NowPlaying } from './features/nowPlaying';

const UiRedirect: FC = () => (
    <Redirect to={'/ui' + window.location.pathname + window.location.search} />
);

function App() {
    return (
        <BrowserRouter>
            <Switch>
                <Route path="/ui/word/:wordId">
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
                <Route path="/ui/*">
                    <Redirect to="/ui/nowPlaying" />
                </Route>
                <UiRedirect />
            </Switch>
        </BrowserRouter>
    );
}

export default App;

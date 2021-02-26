import React, { FC } from 'react';
import { Redirect, Route, Switch } from 'react-router-dom';
import { SearchWordPage, WordPage } from '../../pages/wordPage';
import { DialogPage } from '../../pages/dialogPage';
import { RankedWordsPage } from '../../pages/rankedWordsPage';
import { InfoPage } from '../../pages/infoPage';
import { NowPlaying } from '../nowPlaying';
import { StylesPage } from '../engDialog/stylesPage';
import { WordSearchPage } from 'features/wordDefinition';

const UiRedirect: FC = () => (
    <Redirect to={'/ui' + window.location.pathname + window.location.search} />
);

export const AppSwitch: FC = () => (
    <Switch>
        <Route path="/ui/word/search">
            <WordSearchPage />
        </Route>
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
        <Route path="/ui/engSubs/stylesOf/:showId">
            <StylesPage />
        </Route>
        <Route path="/ui/*">
            <Redirect to="/ui/nowPlaying" />
        </Route>
        <Route>
            <UiRedirect />
        </Route>
    </Switch>
);

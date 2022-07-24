import { AnkiPage } from 'features/anki/ankiPage';
import { WordSearchPage } from 'features/wordDefinition';
import { FC } from 'react';
import { Navigate, Route, Routes } from 'react-router-dom';
import { DialogPage } from '../../pages/dialogPage';
import { InfoPage } from '../../pages/infoPage';
import { RankedWordsPage } from '../../pages/rankedWordsPage';
import { SearchWordPage, WordPage } from '../../pages/wordPage';
import { StylesPage } from '../engDialog/stylesPage';
import { NowPlaying } from '../nowPlaying';

export const AppSwitch: FC = () => (
    <Routes>
        <Route path="/ui">
            <Route path="word">
                <Route path="search" element={<WordSearchPage />} />
                <Route path=":wordId" element={<WordPage />} />
                <Route element={<SearchWordPage />} />
            </Route>
            <Route path="dialog" element={<DialogPage />} />
            <Route path="rankedWords/:pageNum?" element={<RankedWordsPage />} />
            <Route path="settings" element={<InfoPage />} />
            <Route path="nowPlaying" element={<NowPlaying />} />
            <Route path="engSubs/stylesOf/:showId" element={<StylesPage />} />
            <Route path="anki" element={<AnkiPage />} />
            <Route path="*" element={<Navigate replace to="nowPlaying" />} />
        </Route>
        <Route
            element={
                <Navigate
                    replace
                    to={
                        '/ui' +
                        window.location.pathname +
                        window.location.search
                    }
                />
            }
        />
    </Routes>
);

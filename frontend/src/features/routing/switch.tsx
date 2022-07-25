import { AnkiPage } from 'features/anki/ankiPage';
import { WordSearchPage } from 'features/wordDefinition';
import { NotFoundPage } from 'pages/notFoundPage';
import { FC } from 'react';
import { Navigate, Route, Routes, useLocation } from 'react-router-dom';
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
                <Route path="" element={<SearchWordPage />} />
            </Route>
            <Route path="dialog" element={<DialogPage />} />
            <Route path="rankedWords">
                <Route path=":pageNum" element={<RankedWordsPage />} />
                <Route path="" element={<RankedWordsPage />} />
            </Route>
            <Route path="settings" element={<InfoPage />} />
            <Route path="nowPlaying" element={<NowPlaying />} />
            <Route path="engSubs/stylesOf/:showId" element={<StylesPage />} />
            <Route path="anki" element={<AnkiPage />} />
            <Route path="" element={<Navigate replace to="nowPlaying" />} />
            <Route path="*" element={<NotFoundPage />} />
        </Route>
        <Route path="*" element={<UiRedirect />} />
    </Routes>
);

const UiRedirect: FC = () => {
    const location = useLocation();

    return (
        <Navigate replace to={'/ui' + location.pathname + location.search} />
    );
};

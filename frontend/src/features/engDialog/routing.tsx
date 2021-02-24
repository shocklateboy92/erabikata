import { Route } from 'react-router-dom';
import { FC } from 'react';
import { StylesPage } from './stylesPage';

export const StylesPageRoute: FC = () => (
    <Route path="/ui/engSubs/stylesOf/:showId">
        <StylesPage />
    </Route>
);

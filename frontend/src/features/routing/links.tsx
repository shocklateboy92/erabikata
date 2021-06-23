import { useEncodedSelectionParams } from 'features/selectedWord/api';
import React, { FC } from 'react';
import { Link } from 'react-router-dom';

export const StylesOfPageLink: FC<{ showId: number }> = ({
    children,
    showId
}) => <Link to={`/ui/engSubs/stylesOf/${showId}`}>{children}</Link>;

export const AnkiPageLink: FC = ({ children }) => {
    const search = useEncodedSelectionParams();
    return <Link to={{ pathname: '/ui/anki', search }}>{children}</Link>;
};

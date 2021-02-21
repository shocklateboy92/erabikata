import React, { FC } from 'react';
import { Redirect, useParams } from 'react-router-dom';
import { Page } from 'components/page';
import { SelectedWord } from 'features/selectedWord';
import { FullPageError } from 'components/fullPageError';
import { WordOccurrences } from '../features/wordContext/occurrences';

export const WordPage: FC = () => {
    const { wordId } = useParams<{
        wordId: string;
    }>();

    const parsedId = parseInt(wordId);
    if (!parsedId) {
        return <FullPageError>Error: Invalid word id: {wordId}</FullPageError>;
    }

    return (
        <Page title={wordId} secondaryChildren={() => <SelectedWord />}>
            <WordOccurrences wordId={parsedId} />
        </Page>
    );
};

export const SearchWordPage: FC = () => {
    const params = new URLSearchParams(window.location.search);
    const word = params.get('word');

    if (!word) {
        return <FullPageError>Invalid Word '{word}'</FullPageError>;
    }

    return <Redirect to={`/ui/word/${word}`} />;
};

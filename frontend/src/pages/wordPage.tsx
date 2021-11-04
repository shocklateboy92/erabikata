import { useTypedSelector } from 'app/hooks';
import { FullPageError } from 'components/fullPageError';
import { Page } from 'components/page';
import { SelectedWord, wordSelectionV2 } from 'features/selectedWord';
import React, { FC } from 'react';
import { selectWordDefinition } from 'features/wordDefinition/selectors';
import { Redirect, useParams } from 'react-router-dom';
import { WordOccurrences } from '../features/wordContext/occurrences';
import { useAppDispatch } from 'app/store';

export const WordPage: FC = () => {
    const { wordId } = useParams<{
        wordId: string;
    }>();

    const parsedId = parseInt(wordId);
    const title = useTypedSelector(
        (state) =>
            selectWordDefinition(state, parsedId)?.japanese[0].kanji ?? wordId
    );
    if (!parsedId) {
        return <FullPageError>Error: Invalid word id: {wordId}</FullPageError>;
    }

    return (
        <Page title={title} secondaryChildren={() => <SelectedWord />}>
            <WordOccurrences wordId={parsedId} />
        </Page>
    );
};

export const SearchWordPage: FC = () => {
    const params = new URLSearchParams(window.location.search);
    const word = params.get('word');
    const dispatch = useAppDispatch();

    if (word) {
        const wordId =
            parseInt(word) ||
            parseInt(/https?:\/\/takoboto\.jp\/?\?w=(\d+)/.exec(word)?.[1]!);

        if (wordId) {
            dispatch(wordSelectionV2([wordId]));
            return <Redirect to={`/ui/word/${wordId}?word=${wordId}`} />;
        }
    }

    return <Redirect to={`/ui/word/search?query=${word}`} />;
};

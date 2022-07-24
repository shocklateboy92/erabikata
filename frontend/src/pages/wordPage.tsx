import { useTypedSelector } from 'app/hooks';
import { useAppDispatch } from 'app/store';
import { FullPageError } from 'components/fullPageError';
import { Page } from 'components/page';
import { wordSelectionV2 } from 'features/selectedWord';
import { selectWordDefinition } from 'features/wordDefinition/selectors';
import { FC } from 'react';
import { Navigate, useParams } from 'react-router-dom';
import { WordOccurrences } from '../features/wordContext/occurrences';

export const WordPage: FC = () => {
    const { wordId } = useParams();

    const parsedId = parseInt(wordId!);
    const title = useTypedSelector(
        (state) =>
            selectWordDefinition(state, parsedId)?.japanese[0].kanji ?? wordId
    );
    if (!parsedId) {
        return <FullPageError>Error: Invalid word id: {wordId}</FullPageError>;
    }

    return (
        <Page title={title}>
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
            return (
                <Navigate replace to={`/ui/word/${wordId}?word=${wordId}`} />
            );
        }
    }

    return <Navigate replace to={`/ui/word/search?query=${word}`} />;
};

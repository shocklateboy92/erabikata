import { useAppDispatch } from 'app/store';
import { useWordsSearchQuery } from 'backend';
import { FC, useEffect } from 'react';
import { FullWidthText } from '../../components/fullWidth';
import { Page } from '../../components/page';
import { QueryPlaceholder } from '../../components/placeholder/queryPlaceholder';
import { Definition } from './component';
import { fetchDefinitionsIfNeeded } from './slice';

const SearchResults: FC<{ query: string }> = ({ query }) => {
    const response = useWordsSearchQuery({ query });

    const dispatch = useAppDispatch();
    useEffect(() => {
        if (response.data) {
            dispatch(fetchDefinitionsIfNeeded({ wordId: response.data }));
        }
    });

    if (!response.data) {
        return <QueryPlaceholder result={response} />;
    }
    return (
        <>
            {response.data.map((wordId) => (
                <Definition wordId={wordId} />
            ))}
        </>
    );
};
export const WordSearchPage: FC = () => {
    const query = new URLSearchParams(window.location.search).get('query');

    return (
        <Page title={query + ' search results'}>
            <FullWidthText>TODO: Add search bar</FullWidthText>
            {query && <SearchResults query={query} />}
        </Page>
    );
};

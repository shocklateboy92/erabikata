import { mdiFuriganaHorizontal, mdiPageNextOutline, mdiSend } from '@mdi/js';
import Icon from '@mdi/react';
import { useAppDispatch } from 'app/store';
import { toggleWordFurigana } from 'features/furigana';
import { AnkiPageLink } from 'features/routing/links';
import { FC, useEffect } from 'react';
import { useTypedSelector } from '../../app/hooks';
import { Drawer } from '../../features/drawer';
import { selectSelectedEpisodeId, selectSelectedWords } from '../selectedWord';
import { Definition } from './component';
import { ExternalDictionaryLink } from './externalDictionaryLink';
import { fetchDefinitionsIfNeeded, readingsOnlyModeToggle } from './slice';

export const WordDefinitionDrawer: FC<{
    exact?: boolean;
    initiallyOpen: boolean;
    toggleDefinition?: boolean;
}> = ({ initiallyOpen, exact, toggleDefinition }) => {
    const selectedEpisode = useTypedSelector(selectSelectedEpisodeId);
    const wordIds = useTypedSelector(selectSelectedWords);

    const dispatch = useAppDispatch();
    useEffect(() => {
        dispatch(fetchDefinitionsIfNeeded({ wordId: wordIds }));
    }, [wordIds, dispatch]);

    const definition = exact ? wordIds.slice(0, 1) : wordIds.slice(1);
    return (
        <Drawer
            summary={exact ? 'Definition' : 'Related Words'}
            extraActions={() => (
                <>
                    {exact && (
                        <AnkiPageLink>
                            <Icon path={mdiSend} />
                        </AnkiPageLink>
                    )}
                    {toggleDefinition && (
                        <button
                            onClick={() => dispatch(readingsOnlyModeToggle())}
                        >
                            <Icon path={mdiPageNextOutline} />
                        </button>
                    )}
                    {exact && (
                        <button
                            onClick={() => {
                                dispatch(toggleWordFurigana(definition[0]));
                            }}
                        >
                            <Icon path={mdiFuriganaHorizontal} />
                        </button>
                    )}
                    {exact && <ExternalDictionaryLink wordId={definition[0]} />}
                </>
            )}
            startOpen={initiallyOpen}
        >
            {definition.map((word) => (
                <Definition
                    isPrimary={exact}
                    key={word}
                    wordId={word}
                    episodeId={selectedEpisode}
                />
            ))}
        </Drawer>
    );
};

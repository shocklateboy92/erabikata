import { mdiFuriganaHorizontal, mdiPageNextOutline, mdiSend } from '@mdi/js';
import Icon from '@mdi/react';
import { toggleWordFurigana } from 'features/furigana';
import { AnkiPageLink } from 'features/routing/links';
import React, { FC, useEffect } from 'react';
import { useDispatch } from 'react-redux';
import { useTypedSelector } from '../../app/hooks';
import { Drawer } from '../../components/drawer';
import { selectSelectedEpisodeId, selectSelectedWords } from '../selectedWord';
import { Definition } from './component';
import {
    fetchDefinitionsIfNeeded,
    fetchEpisodeRanksIfNeeded,
    readingsOnlyModeToggle
} from './slice';

export const WordDefinitionDrawer: FC<{
    exact?: boolean;
    initiallyOpen: boolean;
    toggleDefinition?: boolean;
}> = ({ initiallyOpen, exact, toggleDefinition }) => {
    const selectedEpisode = useTypedSelector(selectSelectedEpisodeId);
    const wordIds = useTypedSelector(selectSelectedWords);

    const dispatch = useDispatch();
    useEffect(() => {
        dispatch(fetchDefinitionsIfNeeded(wordIds));
        selectedEpisode &&
            dispatch(fetchEpisodeRanksIfNeeded([selectedEpisode, wordIds]));
    }, [wordIds, selectedEpisode, dispatch]);

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
                                dispatch(toggleWordFurigana(definition));
                            }}
                        >
                            <Icon path={mdiFuriganaHorizontal} />
                        </button>
                    )}
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

import React, { FC, useEffect } from 'react';
import { useTypedSelector } from '../../app/hooks';
import { selectSelectedEpisodeId, selectSelectedWords } from '../selectedWord';
import { useDispatch } from 'react-redux';
import {
    fetchDefinitionsIfNeeded,
    fetchEpisodeRanksIfNeeded,
    readingsOnlyModeToggle
} from './slice';
import { Drawer } from '../../components/drawer';
import Icon from '@mdi/react';
import { mdiPageNextOutline, mdiSend } from '@mdi/js';
import { Definition } from './component';
import { AnkiPageLink } from 'features/routing/links';

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
            extraActions={(iconSize) => (
                <>
                    {exact && (
                        <AnkiPageLink>
                            <Icon size={iconSize} path={mdiSend} />
                        </AnkiPageLink>
                    )}
                    {toggleDefinition && (
                        <button
                            onClick={() => dispatch(readingsOnlyModeToggle())}
                        >
                            <Icon path={mdiPageNextOutline} size={iconSize} />
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

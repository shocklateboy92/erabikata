import React, { FC } from 'react';
import { useTypedSelector } from '../../app/hooks';
import { selectSelectedWord } from '../selectedWord';
import { Drawer } from '../../components/drawer';
import { EngDialogList } from './engDialogList';
import { useEngSubsShowIdOfQuery } from 'backend';
import { StylesOfPageLink } from '../routing/links';
import Icon from '@mdi/react';
import { mdiShare } from '@mdi/js';

export const EngDialogDrawer: FC = () => {
    const { episode, sentenceTimestamp } = useTypedSelector(selectSelectedWord);
    const { data: showId } = useEngSubsShowIdOfQuery(
        { episodeId: episode! },
        { skip: !episode }
    );
    if (!(episode && sentenceTimestamp && showId)) {
        return null;
    }

    return (
        <Drawer
            summary="English Context"
            extraActions={(iconSize) => (
                <StylesOfPageLink showId={showId}>
                    <Icon path={mdiShare} size={iconSize} />
                </StylesOfPageLink>
            )}
        >
            <EngDialogList episodeId={episode} time={sentenceTimestamp} />
        </Drawer>
    );
};

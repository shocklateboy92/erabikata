import React, { FC } from 'react';
import { Drawer } from '../../components/drawer';
import { Link } from 'react-router-dom';
import { encodeSelectionParams, selectSelectedWord } from '../selectedWord';
import Icon from '@mdi/react';
import { mdiShare } from '@mdi/js';
import { DialogList } from './dialogList';
import { useTypedSelector } from '../../app/hooks';

export const DialogDrawer: FC = () => {
    const {
        wordIds,
        sentenceTimestamp: time,
        episode: episode
    } = useTypedSelector(selectSelectedWord);
    if (!(time && episode)) {
        return null;
    }

    return (
        <Drawer
            summary="Dialog Context"
            extraActions={(iconSize) => (
                <Link
                    to={{
                        pathname: '/dialog',
                        search: encodeSelectionParams(episode, time, wordIds)
                    }}
                >
                    <Icon path={mdiShare} size={iconSize} />
                </Link>
            )}
        >
            {<DialogList episode={episode} time={time} count={2} />}
        </Drawer>
    );
};

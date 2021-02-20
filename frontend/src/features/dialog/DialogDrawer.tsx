import React, { FC } from 'react';
import { Drawer } from '../../components/drawer';
import { Link } from 'react-router-dom';
import { encodeSelectionParams, selectSelectedWord } from '../selectedWord';
import Icon from '@mdi/react';
import { mdiShare, mdiShareVariant } from '@mdi/js';
import { DialogList } from './dialogList';
import { useTypedSelector } from '../../app/hooks';
import { InlineButton } from '../../components/button';
import { shareSelectedWordDialog } from '../selectedWord/api';
import { useDispatch } from 'react-redux';

export const DialogDrawer: FC = () => {
    const dispatch = useDispatch();
    const { wordIds, sentenceTimestamp: time, episode } = useTypedSelector(
        selectSelectedWord
    );
    if (!(time && episode)) {
        return null;
    }

    return (
        <Drawer
            summary="Dialog Context"
            extraActions={(iconSize) => [
                <InlineButton
                    key="share"
                    onClick={() => {
                        dispatch(shareSelectedWordDialog());
                    }}
                >
                    <Icon path={mdiShareVariant} size={iconSize} />
                </InlineButton>,
                <Link
                    key="link"
                    to={{
                        pathname: '/dialog',
                        search: encodeSelectionParams(episode, time, wordIds)
                    }}
                >
                    <Icon path={mdiShare} size={iconSize} />
                </Link>
            ]}
        >
            {<DialogList episode={episode} time={time} count={2} />}
        </Drawer>
    );
};

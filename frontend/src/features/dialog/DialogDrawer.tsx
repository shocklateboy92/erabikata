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
import { HassPlayButton } from '../hass';

export const DialogDrawer: FC = () => {
    const dispatch = useDispatch();
    const {
        wordIds,
        sentenceTimestamp: time,
        episode,
        wordBaseForm
    } = useTypedSelector(selectSelectedWord);
    if (!(time && episode)) {
        return null;
    }

    return (
        <Drawer
            summary="Dialog Context"
            extraActions={(iconSize) => [
                <HassPlayButton
                    key="hass"
                    dialogId={time}
                    episodeId={episode}
                    iconSize={iconSize}
                />,
                <Link
                    key="link"
                    to={{
                        pathname: '/dialog',
                        search: encodeSelectionParams(
                            episode,
                            time,
                            wordIds,
                            wordBaseForm
                        )
                    }}
                >
                    <Icon path={mdiShare} size={iconSize} />
                </Link>,
                <InlineButton
                    key="share"
                    onClick={() => {
                        dispatch(shareSelectedWordDialog());
                    }}
                >
                    <Icon path={mdiShareVariant} size={iconSize} />
                </InlineButton>
            ]}
        >
            {<DialogList episode={episode} time={time} count={2} />}
        </Drawer>
    );
};

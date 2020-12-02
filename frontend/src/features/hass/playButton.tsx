import { mdiPlayNetwork } from '@mdi/js';
import Icon from '@mdi/react';
import { useAppSelector } from 'app/hooks';
import { selectIsPlayingInSelectedPlayer } from 'features/hass';
import React, { FC } from 'react';
import { useDispatch } from 'react-redux';
import { playFrom, useHass } from './api';

export const HassPlayButton: FC<{
    episodeId: string;
    dialogId: number;
    iconSize?: number;
}> = (props) => {
    const [dispatch, context] = [useDispatch(), useHass()];
    const shouldShow = useAppSelector((state) =>
        selectIsPlayingInSelectedPlayer(state, props.episodeId)
    );

    if (!shouldShow) {
        return null;
    }

    return (
        <button
            onClick={() =>
                dispatch(
                    playFrom({
                        context,
                        timeStamp: Math.floor(props.dialogId - 1)
                    })
                )
            }
        >
            <Icon path={mdiPlayNetwork} size={props.iconSize} />
        </button>
    );
};

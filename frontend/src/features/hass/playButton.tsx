import { mdiPlayNetwork } from '@mdi/js';
import Icon from '@mdi/react';
import { useAppSelector } from 'app/hooks';
import { useAppDispatch } from 'app/store';
import { selectIsPlayingInSelectedPlayer } from 'features/hass';
import { FC } from 'react';
import { playFrom } from './api';

export const HassPlayButton: FC<{
    episodeId?: string;
    dialogId?: number;
    iconSize?: string;
}> = (props) => {
    const dispatch = useAppDispatch();
    const shouldShow =
        useAppSelector((state) =>
            selectIsPlayingInSelectedPlayer(state, props.episodeId)
        ) &&
        props.episodeId &&
        props.dialogId;

    if (!shouldShow) {
        return null;
    }

    return (
        <button
            onClick={() =>
                dispatch(
                    playFrom({
                        timeStamp: Math.floor(props.dialogId! - 1)
                    })
                )
            }
        >
            <Icon path={mdiPlayNetwork} size={props.iconSize} />
        </button>
    );
};

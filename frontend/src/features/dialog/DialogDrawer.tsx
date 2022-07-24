import { mdiShare, mdiShareVariant } from '@mdi/js';
import Icon from '@mdi/react';
import { useAppDispatch } from 'app/store';
import { selectEpisodeTitle } from 'features/dialog/selectors';
import { FC } from 'react';
import { Link } from 'react-router-dom';
import { useTypedSelector } from '../../app/hooks';
import { InlineButton } from '../../components/button';
import { Drawer } from '../../features/drawer';
import { HassPlayButton } from '../hass';
import { encodeSelectionParams, selectSelectedWord } from '../selectedWord';
import { shareSelectedWordDialog } from '../selectedWord/api';
import { DialogList } from './dialogList';

export const DialogDrawer: FC = () => {
    const dispatch = useAppDispatch();
    const {
        wordIds,
        sentenceTimestamp: time,
        episode
    } = useTypedSelector(selectSelectedWord);
    const episodeTitle = useTypedSelector(
        (state) => episode && selectEpisodeTitle(state, episode)
    );
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
                        pathname: '/ui/dialog',
                        search: encodeSelectionParams(episode, time, wordIds)
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
            <h3 className="dialog-episode-title">{episodeTitle}</h3>
            <DialogList episode={episode} time={time} count={2} />
        </Drawer>
    );
};

import { useTypedSelector } from 'app/hooks';
import { FullWidthText } from 'components/fullWidth';
import { Page } from 'components/page';
import { DialogList } from 'features/dialog/dialogList';
import { selectIsPlayerSelected, selectSelectedPlayer } from 'features/hass';
import { SelectedWord } from 'features/selectedWord';
import React, { FC, useEffect } from 'react';
import { useDispatch } from 'react-redux';
import { Link } from 'react-router-dom';
import {
    nowPlayingPositionUpdateRequest,
    selectNowPlayingMediaTimeStamp
} from './slice';

export const NowPlaying: FC = () => {
    const dispatch = useDispatch();
    useEffect(() => {
        dispatch(nowPlayingPositionUpdateRequest());
    }, [dispatch]);
    const isPlayerSelected = useTypedSelector(selectIsPlayerSelected);
    const session = useTypedSelector(selectSelectedPlayer);
    const time = useTypedSelector(selectNowPlayingMediaTimeStamp);

    if (!session) {
        if (isPlayerSelected) {
            return (
                <Page>
                    <FullWidthText>
                        字幕はまもなくここに出てきます！
                        <br />
                        少々お待ちください。
                    </FullWidthText>
                </Page>
            );
        }

        return (
            <Page>
                <FullWidthText>
                    <Link to="/settings">プレイヤーをセレクトしてね！</Link>
                </FullWidthText>
            </Page>
        );
    }

    if (!session.media) {
        return (
            <Page>
                <FullWidthText>今、何も再生されていません。</FullWidthText>
            </Page>
        );
    }

    return (
        <Page
            secondaryChildren={() => <SelectedWord />}
            title={session.media.title}
        >
            <DialogList
                count={3}
                episode={session.media.id.toString()}
                time={time!}
            />
        </Page>
    );
};

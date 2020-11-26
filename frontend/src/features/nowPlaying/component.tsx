import { getToken } from 'api/plexToken';
import { useAppSelector } from 'app/hooks';
import { FullWidthText } from 'components/fullWidth';
import { Page } from 'components/page';
import { selectNearbyDialog } from 'features/dialog/slice';
import { SelectedWord } from 'features/selectedWord';
import React, { FC, useEffect } from 'react';
import { shallowEqual, useDispatch, useSelector } from 'react-redux';
import { Redirect } from 'react-router-dom';
import { Dialog } from '../dialog/Dialog';
import {
    fetchNowPlayingSessions,
    selectNowPlayingSessions,
    selectNowPlayingSessionsPending
} from './slice';

const Session: FC<{ episodeId: string; currentTime: number }> = ({
    episodeId,
    currentTime
}) => {
    const dialog = useAppSelector(
        selectNearbyDialog.bind(null, episodeId, currentTime, 3)
    );
    if (!dialog) {
        return null;
    }

    return (
        <>
            {dialog.map((dialog) => (
                <Dialog key={dialog} time={dialog} episode={episodeId} />
            ))}
        </>
    );
};

export const NowPlaying: FC = () => {
    const dispatch = useDispatch();
    useEffect(() => {
        dispatch(fetchNowPlayingSessions());
    }, [dispatch]);

    const pending = useSelector(selectNowPlayingSessionsPending, shallowEqual);
    const [session] = useSelector(selectNowPlayingSessions, shallowEqual);

    const token = getToken();
    if (!token) {
        return <Redirect to="/login" />;
    }

    if (!session) {
        if (pending) {
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
                <FullWidthText>今、何も再生されていません。</FullWidthText>
            </Page>
        );
    }

    return (
        <Page
            secondaryChildren={() => <SelectedWord />}
            title={session.episodeTitle}
        >
            <Session episodeId={session.episodeId} currentTime={session.time} />
        </Page>
    );
};

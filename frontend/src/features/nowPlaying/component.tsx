import { useTypedSelector } from 'app/hooks';
import { FullWidthText } from 'components/fullWidth';
import { Page } from 'components/page';
import { DialogList } from 'features/dialog/dialogList';
import { selectIsPlayerSelected, selectSelectedPlayer } from 'features/hass';
import { SelectedWord } from 'features/selectedWord';
import { WakeLock } from 'features/wakeLock';
import React, { FC } from 'react';
import { Link } from 'react-router-dom';

export const NowPlaying: FC = () => {
    const isPlayerSelected = useTypedSelector(selectIsPlayerSelected);
    const session = useTypedSelector(selectSelectedPlayer);

    if (!isPlayerSelected) {
        return (
            <Page>
                <FullWidthText>
                    <Link to="/ui/settings">プレイヤーをセレクトしてね！</Link>
                </FullWidthText>
            </Page>
        );
    }

    if (!session) {
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
                time={session.media.position}
            />
            <WakeLock />
        </Page>
    );
};

import { useAppSelector, useTypedSelector } from 'app/hooks';
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
    const media = useAppSelector((state) => selectSelectedPlayer(state)?.media);

    if (!isPlayerSelected) {
        return (
            <Page>
                <FullWidthText>
                    <Link to="/ui/settings">プレイヤーをセレクトしてね！</Link>
                </FullWidthText>
            </Page>
        );
    }

    if (!media) {
        return (
            <Page>
                <FullWidthText>今、何も再生されていません。</FullWidthText>
            </Page>
        );
    }

    return (
        <Page secondaryChildren={() => <SelectedWord />} title={media.title}>
            <DialogList
                count={5}
                episode={media.id.toString()}
                time={media.position}
                autoSelectNearest={
                    // to avoid selecting on page navigation etc.
                    new Date().getTime() -
                        new Date(media.position_last_updated_at).getTime() <
                    1000
                }
            />
            <WakeLock />
        </Page>
    );
};

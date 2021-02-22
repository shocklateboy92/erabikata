import { useTypedSelector } from 'app/hooks';
import { FullPageError } from 'components/fullPageError';
import { Page } from 'components/page';
import { DialogList } from 'features/dialog/dialogList';
import { selectEpisodeTitle } from 'features/dialog/selectors';
import { SelectedWord } from 'features/selectedWord';
import React, { FC } from 'react';
import { useLocation } from 'react-router-dom';

export const DialogPage: FC = () => {
    const { search } = useLocation();

    const params = new URLSearchParams(search);
    const episode = params.get('episode');
    const time = parseFloat(params.get('time')!);
    const title = useTypedSelector((state) =>
        selectEpisodeTitle(state, episode)
    );

    if (!episode || !Number.isFinite(time)) {
        return <FullPageError>Invalid EpisodeId and/or Time</FullPageError>;
    }
    return (
        <Page title={title} secondaryChildren={() => <SelectedWord />}>
            <DialogList episode={episode} time={time} count={8} />
        </Page>
    );
};

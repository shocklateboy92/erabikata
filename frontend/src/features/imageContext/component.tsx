import { mdiSubtitlesOutline } from '@mdi/js';
import Icon from '@mdi/react';
import { useTypedSelector } from 'app/hooks';
import { InlineButton } from 'components/button';
import { Drawer } from 'components/drawer';
import { selectBaseUrl } from 'features/backendSelection';
import React, { FC, useState } from 'react';

export const ImageContext: FC<{ episodeId: string; time: number }> = ({
    episodeId,
    time
}) => {
    const baseUrl = useTypedSelector(selectBaseUrl);
    const [includeSubs, setIncludeSubs] = useState(false);
    return (
        <Drawer
            summary="Image Context"
            extraActions={(iconSize) => (
                <InlineButton onClick={() => setIncludeSubs(!includeSubs)}>
                    <Icon path={mdiSubtitlesOutline} size={iconSize} />
                </InlineButton>
            )}
        >
            <img
                src={`${baseUrl}/api/image/${episodeId}/${time}?includeSubs=${includeSubs}`}
                alt="Screenshot of video at selected dialog time"
            />
        </Drawer>
    );
};

import { useTypedSelector } from 'app/hooks';
import { Drawer } from 'components/drawer';
import { selectBaseUrl } from 'features/backendSelection';
import React, { FC } from 'react';

export const ImageContext: FC<{ episodeId: string; time: number }> = ({
    episodeId,
    time
}) => {
    const baseUrl = useTypedSelector(selectBaseUrl);
    return (
        <Drawer summary="Image Context">
            <img
                src={`${baseUrl}/api/image/${episodeId}/${time}`}
                alt="Screenshot of video at selected dialog time"
            />
        </Drawer>
    );
};

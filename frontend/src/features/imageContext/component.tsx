import { mdiSubtitlesOutline } from '@mdi/js';
import Icon from '@mdi/react';
import { useTypedSelector } from 'app/hooks';
import classNames from 'classnames';
import { InlineButton } from 'components/button';
import { Drawer } from 'components/drawer';
import { Spinner, SpinnerContainer } from 'components/spinner';
import { selectBaseUrl } from 'features/backendSelection';
import React, { FC, useEffect, useLayoutEffect, useState } from 'react';
import styles from './imageContext.module.scss';

export const ImageContext: FC<{ episodeId: string; time: number }> = ({
    episodeId,
    time
}) => {
    const baseUrl = useTypedSelector(selectBaseUrl);
    const [includeSubs, setIncludeSubs] = useState(false);
    const [isLoading, setLoading] = useState(true);
    const [showSpinner, setShowSpinner] = useState(false);

    // This is a 2 step update to show the spinner
    // First, set Loading to true when the image src changes
    // 100ms after that, set showSpinner to true
    // Set Loading to false once load() fires on the img
    // This requires like 4 re-renders per image load.
    // TODO: Find a better way of doing this
    useLayoutEffect(() => {
        setShowSpinner(false);
        setLoading(true);
    }, [episodeId, time, includeSubs]);
    useEffect(() => {
        window.setTimeout(() => setShowSpinner(isLoading), 100);
    }, [isLoading]);

    return (
        <Drawer
            summary="Image Context"
            extraActions={(iconSize) => (
                <InlineButton
                    onClick={() => {
                        setIncludeSubs(!includeSubs);
                    }}
                >
                    <Icon path={mdiSubtitlesOutline} size={iconSize} />
                </InlineButton>
            )}
        >
            <SpinnerContainer>
                <img
                    className={classNames(styles.image, {
                        [styles.loading]: isLoading && showSpinner
                    })}
                    onLoad={() => setLoading(false)}
                    width="1920"
                    height="1080"
                    src={`${baseUrl}/api/image/${episodeId}/${time}?includeSubs=${includeSubs}`}
                    alt="Screenshot of video at selected dialog time"
                />
                {isLoading && showSpinner && <Spinner />}
            </SpinnerContainer>
        </Drawer>
    );
};

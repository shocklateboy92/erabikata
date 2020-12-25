import { mdiSubtitlesOutline } from '@mdi/js';
import Icon from '@mdi/react';
import { useTypedSelector } from 'app/hooks';
import classNames from 'classnames';
import { InlineButton } from 'components/button';
import { Drawer } from 'components/drawer';
import { Spinner, SpinnerContainer } from 'components/spinner';
import { selectBaseUrl } from 'features/backendSelection';
import React, { FC, useEffect, useRef, useState } from 'react';
import styles from './imageContext.module.scss';

export const ImageContext: FC<{ episodeId: string; time: number }> = ({
    episodeId,
    time
}) => {
    const baseUrl = useTypedSelector(selectBaseUrl);
    const [includeSubs, setIncludeSubs] = useState(false);
    const [showSpinner, setShowSpinner] = useState(false);

    const srcUrl = `${baseUrl}/api/image/${episodeId}/${time}?includeSubs=${includeSubs}`;

    // Create a ref to store our timer in
    const timer = useRef(0);
    useEffect(() => {
        // Whenever the srcUrl changes, we're loading a new image
        // If it takes more than 100ms, start showing the spinner
        timer.current = window.setTimeout(() => setShowSpinner(true), 100);

        // In case we unmount before the timer triggers
        return () => clearTimeout(timer.current);
    }, [srcUrl, timer]);

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
                        [styles.loading]: showSpinner
                    })}
                    onLoad={() => {
                        // Cancel the timer when the image loads. That way, if
                        // it's already in the browser cache and loades before
                        // 100ms elapses, we never show the spinner.
                        clearTimeout(timer.current);
                        setShowSpinner(false);
                    }}
                    width="1920"
                    height="1080"
                    src={srcUrl}
                    alt="Screenshot of video at selected dialog time"
                />
                {showSpinner && <Spinner />}
            </SpinnerContainer>
        </Drawer>
    );
};

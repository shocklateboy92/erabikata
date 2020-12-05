import { useTypedSelector } from 'app/hooks';
import { selectIsCurrentPlayerActive } from 'features/hass';
import React, { useEffect } from 'react';
import { FC } from 'react';

let wakeLock: any = null;
const release = () => {
    if (!wakeLock) {
        return;
    }

    console.log('Releasing wakeLock');
    wakeLock.release();
    wakeLock = null;
};

export const WakeLock: FC = () => {
    const active = useTypedSelector(selectIsCurrentPlayerActive);
    const enabled = useTypedSelector((state) => state.wakeLock.enabled);
    useEffect(() => {
        if (active && enabled && 'wakeLock' in navigator) {
            (async () => {
                // create an async function to request a wake lock
                try {
                    if (wakeLock) {
                        return;
                    }
                    console.log('Acquiring wakeLock');
                    wakeLock = await navigator.wakeLock.request('screen');
                } catch (err) {}
            })();
        } else {
            release();
        }

        return release;
    }, [active, enabled]);

    return <></>;
};

// wakeLock is an experimental api
declare global {
    interface Navigator {
        wakeLock?: any;
    }
}

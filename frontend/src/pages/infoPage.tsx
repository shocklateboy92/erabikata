import { mdiRefresh } from '@mdi/js';
import { ActionButton } from 'components/button/actionButton';
import { FullWidthText } from 'components/fullWidth';
import { Page } from 'components/page';
import { BackendInfo } from 'features/backendSelection';
import { AnalyzerSelector } from 'features/backendSelection/analyzerSelector';
import { HassCheck } from 'features/hass';
import { WakeLockOption } from 'features/wakeLock';
import preval from 'preval.macro';
import React, { FC, useEffect, useState } from 'react';
export const InfoPage: FC = () => {
    const [canUpdate, setCanUpdate] = useState(false);
    useEffect(() => {
        navigator.serviceWorker.ready.then((worker) =>
            setCanUpdate(!!worker.waiting)
        );
    }, []);

    return (
        <Page>
            <FullWidthText>
                <h2>App Info</h2>
                Build Version
                <br />
                {preval`module.exports = new Date().toLocaleString();`}
                <p>
                    Backend Url
                    <br />
                    <BackendInfo />
                </p>
                <AnalyzerSelector />
                {canUpdate && (
                    <ActionButton
                        icon={mdiRefresh}
                        onClick={async () => {
                            const worker = await navigator.serviceWorker.ready;
                            worker.waiting?.postMessage({
                                type: 'SKIP_WAITING'
                            });
                        }}
                    >
                        Update App
                    </ActionButton>
                )}
            </FullWidthText>
            <HassCheck />
            <FullWidthText>
                <h2>Playback Options</h2>
                <WakeLockOption />
            </FullWidthText>
        </Page>
    );
};

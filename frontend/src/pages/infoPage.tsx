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
        // event handler is defined separately so it can be unregistered
        const eventHandler = () => setCanUpdate(true);

        navigator.serviceWorker.ready.then((worker) => {
            worker.addEventListener('updatefound', eventHandler);
        });

        return () => {
            navigator.serviceWorker.ready.then((worker) =>
                worker.removeEventListener('updatefound', eventHandler)
            );
        };
    }, []);

    useEffect(() => {
        // kick off the check for updates super frequently.
        navigator.serviceWorker.ready.then((worker) => worker.update());
    });

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
                {canUpdate && (
                    <ActionButton
                        icon={mdiRefresh}
                        onClick={async () => {
                            const worker = await navigator.serviceWorker.ready;
                            const newWorker = worker.waiting;
                            newWorker?.addEventListener(
                                'statechange',
                                () => {
                                    // If the previously waiting worker became the active,
                                    // the update succeeded. Refresh the page.
                                    if (worker.active === newWorker) {
                                        window.location.reload();
                                    }
                                },
                                { once: true }
                            );

                            // Tell the new worker to start its update
                            newWorker?.postMessage({
                                type: 'SKIP_WAITING'
                            });
                        }}
                    >
                        Update App
                    </ActionButton>
                )}
            </FullWidthText>
            <AnalyzerSelector />
            <HassCheck />
            <FullWidthText>
                <h2>Playback Options</h2>
                <WakeLockOption />
            </FullWidthText>
        </Page>
    );
};

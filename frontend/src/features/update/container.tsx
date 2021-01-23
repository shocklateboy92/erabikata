import React from 'react';
import { FC, useEffect, useState } from 'react';
import { UpdateComponent } from './presentation';

export const UpdateCheck: FC = () => {
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
        navigator.serviceWorker.ready.then((worker) => {
            if (worker.waiting) {
                setCanUpdate(true);
            }

            worker.update();
        });
    });

    return (
        <UpdateComponent
            canUpdate={canUpdate}
            onUpdateClick={async () => {
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
        />
    );
};

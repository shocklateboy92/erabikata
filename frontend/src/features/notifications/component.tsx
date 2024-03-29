import { mdiClose } from '@mdi/js';
import Icon from '@mdi/react';
import { useTypedSelector } from 'app/hooks';
import store, { useAppDispatch } from 'app/store';
import { FC, useEffect } from 'react';
import styles from './notifications.module.scss';
import { notification, notificationDeactivation } from './slice';

const Notification: FC<{ id: number }> = ({ id }) => {
    const dispatch = useAppDispatch();
    const content = useTypedSelector(
        (state) => state.notifications.content[id!]
    );
    useEffect(() => {
        if (!content) {
            return;
        }

        const timeout = window.setTimeout(() => {
            dispatch(notificationDeactivation(content.id));
        }, 5000);

        return () => {
            window.clearTimeout(timeout);
        };
    }, [content, dispatch]);

    if (!content) {
        return null;
    }

    return (
        <div className={styles.wrapper}>
            <div className={styles.item}>
                <div>
                    {content.title && <h4>{content.title}</h4>}
                    {content.text}
                </div>
                <div
                    onClick={() =>
                        dispatch(notificationDeactivation(content.id))
                    }
                >
                    <Icon path={mdiClose} size={'2em'} />
                </div>
            </div>
        </div>
    );
};

export const NotifcationsView: FC = () => {
    const activeNotifications = useTypedSelector(
        (state) => state.notifications.active
    );
    if (!activeNotifications.length) {
        return null;
    }

    return (
        <div className={styles.container}>
            {activeNotifications.map((n) => (
                <Notification key={n} id={n} />
            ))}
        </div>
    );
};

let num = 0;
window.addEventListener('keypress', (e) => {
    if (e.key === 'e') {
        store.dispatch(
            notification({
                title: 'Test notification ' + num++,
                text: 'The body of the notification. HOpefully this will be longer'
            })
        );
    }
});

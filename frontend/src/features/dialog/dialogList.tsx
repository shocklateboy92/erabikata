import { mdiChevronLeft, mdiChevronRight } from '@mdi/js';
import Icon from '@mdi/react';
import classNames from 'classnames';
import { InlineButton } from 'components/button';
import { Dialog } from 'features/dialog/Dialog';
import React, { FC, useEffect, useState } from 'react';
import { QueryPlaceholder } from '../../components/placeholder/queryPlaceholder';
import { useNearbyDialogQuery } from './api';
import styles from './dialog.module.scss';

export interface IDialogListProps {
    count: number;
    episode: string;
    time: number;
}

export const DialogList: FC<IDialogListProps> = ({ episode, time, count }) => {
    const [timeOverride, setTimeOverride] = useState(time);

    const { response, dialog } = useNearbyDialogQuery(
        episode,
        timeOverride,
        count
    );
    useEffect(() => {
        setTimeOverride(time);
    }, [time]);

    if (!dialog) {
        return <QueryPlaceholder result={response} />;
    }

    return (
        <>
            <BeginScrollButton
                onClick={() => setTimeOverride(dialog[0].time)}
                isLoading={response.isFetching}
            >
                <Icon path={mdiChevronLeft} size="1.5em" />
                Load Previous
            </BeginScrollButton>

            {dialog.map(({ dialogId }) => (
                <Dialog key={dialogId} dialogId={dialogId} />
            ))}

            <BeginScrollButton
                onClick={() => setTimeOverride(dialog[dialog.length - 1].time)}
                isLoading={response.isFetching}
            >
                Load Next
                <Icon path={mdiChevronRight} size="1.5em" />
            </BeginScrollButton>
        </>
    );
};

interface IScrollButtonProps {
    isLoading: boolean;
    onClick: () => void;
}

const BeginScrollButton: FC<IScrollButtonProps> = ({
    onClick,
    isLoading,
    children
}) => {
    return (
        <InlineButton
            className={classNames(styles.scrollButton, {
                [styles.busy]: isLoading
            })}
            complex
            small
            onClick={onClick}
        >
            {children}
        </InlineButton>
    );
};

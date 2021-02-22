import { mdiChevronLeft, mdiChevronRight } from '@mdi/js';
import Icon from '@mdi/react';
import { useTypedSelector } from 'app/hooks';
import { useEpisodeIndexQuery } from 'backend';
import classNames from 'classnames';
import { InlineButton } from 'components/button';
import { Dialog } from 'features/dialog/Dialog';
import React, { FC, useEffect, useState } from 'react';
import styles from './dialog.module.scss';
import { selectAnalyzer } from '../backendSelection';
import { QueryPlaceholder } from '../../components/placeholder/queryPlaceholder';
import { findNearbyDialog } from '../selectedWord';

export interface IDialogListProps {
    count: number;
    episode: string;
    time: number;
}

export const DialogList: FC<IDialogListProps> = ({ episode, time, count }) => {
    const [timeOverride, setTimeOverride] = useState(time);

    const analyzer = useTypedSelector(selectAnalyzer);
    const response = useEpisodeIndexQuery({
        analyzer,
        episodeId: episode
    });
    useEffect(() => {
        setTimeOverride(time);
    }, [time]);

    if (!response.data) {
        return <QueryPlaceholder result={response} />;
    }
    const dialog = findNearbyDialog(response.data.entries, timeOverride, count);

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

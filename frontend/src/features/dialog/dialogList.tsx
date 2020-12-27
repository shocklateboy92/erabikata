import { mdiChevronLeft, mdiChevronRight } from '@mdi/js';
import Icon from '@mdi/react';
import { useAppSelector } from 'app/hooks';
import { useAppDispatch } from 'app/store';
import classNames from 'classnames';
import { InlineButton } from 'components/button';
import { Dialog } from 'features/dialog/Dialog';
import {
    fetchDialogById,
    IDialogId,
    selectNearbyDialog
} from 'features/dialog/slice';
import React, { FC, useEffect, useState } from 'react';
import styles from './dialog.module.scss';

export interface IDialogListProps extends IDialogId {
    count: number;
}

export const DialogList: FC<IDialogListProps> = ({ episode, time, count }) => {
    const [timeOverride, setTimeOverride] = useState(time);

    const dispatch = useAppDispatch();
    useEffect(() => {
        setTimeOverride(time);
        dispatch(fetchDialogById({ episode, time, count }));
    }, [count, dispatch, episode, time]);

    const dialog = useAppSelector(
        selectNearbyDialog.bind(null, episode, timeOverride, count)
    );

    return (
        <>
            <BeginScrollButton
                count={count}
                episode={episode}
                setTimeOverride={setTimeOverride}
                time={dialog[0]}
            >
                <Icon path={mdiChevronLeft} size="1.5em" />
                Load Previous
            </BeginScrollButton>
            {dialog.map((d) => (
                <Dialog key={d} episode={episode} time={d} />
            ))}
            <BeginScrollButton
                time={dialog[dialog.length - 1]}
                setTimeOverride={setTimeOverride}
                episode={episode}
                count={count}
            >
                Load Next
                <Icon path={mdiChevronRight} size="1.5em" />
            </BeginScrollButton>
        </>
    );
};

interface IScrollButtonProps {
    time: number;
    episode: string;
    setTimeOverride: (time: number) => void;
    count: number;
}
const BeginScrollButton: FC<IScrollButtonProps> = ({
    episode,
    setTimeOverride,
    count,
    time,
    children
}) => {
    const dispatch = useAppDispatch();
    const [isLoading, setIsLoading] = useState(false);

    return (
        <InlineButton
            className={classNames(styles.scrollButton, {
                [styles.busy]: isLoading
            })}
            complex
            onClick={async () => {
                setIsLoading(true);
                await dispatch(fetchDialogById({ episode, time, count }));
                setIsLoading(false);
                setTimeOverride(time);
            }}
        >
            {children}
        </InlineButton>
    );
};

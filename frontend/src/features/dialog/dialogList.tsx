import { mdiChevronLeft, mdiChevronRight } from '@mdi/js';
import Icon from '@mdi/react';
import { useTypedSelector } from 'app/hooks';
import { useEpisodeIndexQuery } from 'backend';
import classNames from 'classnames';
import { InlineButton } from 'components/button';
import { Dialog } from 'features/dialog/Dialog';
import { IDialogId } from 'features/dialog/slice';
import React, { FC, useEffect, useState } from 'react';
import styles from './dialog.module.scss';
import { selectAnalyzer } from '../backendSelection';
import { QueryPlaceholder } from '../../components/placeholder/queryPlaceholder';
import { Entry } from '../../backend-rtk.generated';

export interface IDialogListProps extends IDialogId {
    count: number;
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
    const match = binarySearch(response.data.entries, timeOverride);
    const index = Math.max(0, match - count + 1);
    const dialog = response.data.entries.slice(index, index + count * 2 - 1);

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

function binarySearch(
    arr: Entry[],
    target: number,
    lo = 0,
    hi = arr.length - 1
): number {
    if (target < arr[lo].time) {
        return 0;
    }
    if (target > arr[hi].time) {
        return hi;
    }

    const mid = Math.floor((hi + lo) / 2);

    if (hi - lo < 2) {
        return target - arr[lo].time < arr[hi].time - target ? lo : hi;
    } else {
        return target < arr[mid].time
            ? binarySearch(arr, target, lo, mid)
            : target > arr[mid].time
            ? binarySearch(arr, target, mid, hi)
            : mid;
    }
}

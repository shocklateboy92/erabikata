import { mdiChevronLeft, mdiChevronRight } from '@mdi/js';
import Icon from '@mdi/react';
import { useTypedSelector } from 'app/hooks';
import { useSubsIndexQuery } from 'backend';
import classNames from 'classnames';
import { InlineButton } from 'components/button';
import { Dialog } from 'features/dialog/Dialog';
import { IDialogId } from 'features/dialog/slice';
import React, { FC, useEffect, useState } from 'react';
import styles from './dialog.module.scss';
import { selectAnalyzer } from '../backendSelection';
import { FullWidthText } from '../../components/fullWidth';

export interface IDialogListProps extends IDialogId {
    count: number;
}

export const DialogList: FC<IDialogListProps> = ({ episode, time, count }) => {
    const [timeOverride, setTimeOverride] = useState(time);

    const analyzer = useTypedSelector(selectAnalyzer);
    const response = useSubsIndexQuery({
        analyzer,
        count,
        episode,
        time: timeOverride
    });
    useEffect(() => {
        setTimeOverride(time);
    }, [time]);

    if (!response.data) {
        if (response.isLoading) {
            return <FullWidthText>Now Loading...</FullWidthText>;
        }
        return <pre>{JSON.stringify(response.error, null, 2)}</pre>;
    }
    const { dialog, episodeId } = response.data;

    return (
        <>
            <BeginScrollButton
                onClick={() => setTimeOverride(dialog[0].startTime)}
                isLoading={response.isFetching}
            >
                <Icon path={mdiChevronLeft} size="1.5em" />
                Load Previous
            </BeginScrollButton>

            {dialog.map((dialog) => (
                <Dialog
                    key={dialog.id}
                    content={dialog}
                    episodeId={episodeId}
                />
            ))}

            <BeginScrollButton
                onClick={() =>
                    setTimeOverride(dialog[dialog.length - 1].startTime)
                }
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

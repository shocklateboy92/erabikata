import { mdiAngleRight, mdiArrowRight } from '@mdi/js';
import Icon from '@mdi/react';
import { useAppSelector } from 'app/hooks';
import { useAppDispatch } from 'app/store';
import { InlineButton } from 'components/button';
import { Spinner } from 'components/spinner';
import { Dialog } from 'features/dialog/Dialog';
import {
    fetchDialogById,
    IDialogId,
    selectNearbyDialog
} from 'features/dialog/slice';
import React, { FC, useEffect, useState } from 'react';

export interface IDialogListProps extends IDialogId {
    count: number;
}

export const DialogList: FC<IDialogListProps> = ({ episode, time, count }) => {
    const [timeOverride, setTimeOverride] = useState(time);
    const [isLoading, setIsLoading] = useState(false);

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
                iconPath={mdiArrowRight}
            />
            {dialog.map((d) => (
                <Dialog key={d} episode={episode} time={d} />
            ))}
            <InlineButton>Show More</InlineButton>
        </>
    );
};

interface IScrollButtonProps {
    iconPath: string;
    time: number;
    episode: string;
    setTimeOverride: (time: number) => void;
    count: number;
}
const BeginScrollButton: FC<IScrollButtonProps> = ({
    episode,
    iconPath,
    setTimeOverride,
    count,
    time
}) => {
    const dispatch = useAppDispatch();
    const [isLoading, setIsLoading] = useState(false);

    const scrollTo = async () => {
        setIsLoading(true);
        await dispatch(fetchDialogById({ episode, time, count }));
        setIsLoading(false);
        setTimeOverride(time);
    };

    return (
        <InlineButton onClick={scrollTo}>
            {isLoading ? <Spinner /> : <Icon path={iconPath} size="1em" />}
            Show More
        </InlineButton>
    );
};

import React, { FC, useEffect, useState } from 'react';
import { useDispatch } from 'react-redux';
import { useAppSelector } from 'app/hooks';
import { Dialog } from 'features/dialog/Dialog';
import {
    fetchDialogById,
    IDialogId,
    selectNearbyDialog
} from 'features/dialog/slice';
import { InlineButton } from 'components/button';

export interface IDialogListProps extends IDialogId {
    count: number;
}

export const DialogList: FC<IDialogListProps> = ({ episode, time, count }) => {
    const [timeOverride, setTimeOverride] = useState(time);
    useEffect(() => {
        setTimeOverride(time);
    }, [time]);

    const dispatch = useDispatch();
    useEffect(() => {
        dispatch(fetchDialogById({ episode, time: timeOverride, count }));
    }, [dispatch, episode, timeOverride, count]);

    const dialog = useAppSelector(
        selectNearbyDialog.bind(null, episode, timeOverride, count)
    );

    return (
        <>
            <InlineButton onClick={() => setTimeOverride(dialog[0])}>
                Show More
            </InlineButton>
            {dialog.map((d) => (
                <Dialog key={d} episode={episode} time={d} />
            ))}
            <InlineButton onClick={() => setTimeOverride(dialog[-1])}>
                Show More
            </InlineButton>
        </>
    );
};

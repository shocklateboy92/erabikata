import React, { FC, useEffect } from 'react';
import { useDispatch } from 'react-redux';
import { useAppSelector } from 'app/hooks';
import { Dialog } from 'features/dialog/Dialog';
import {
    fetchDialogById,
    IDialogId,
    selectNearbyDialog
} from 'features/dialog/slice';

export interface IDialogListProps extends IDialogId {
    count: number;
}

export const DialogList: FC<IDialogListProps> = ({ episode, time, count }) => {
    const dispatch = useDispatch();
    useEffect(() => {
        dispatch(fetchDialogById({ episode, time, count }));
    }, [dispatch, episode, time, count]);

    const dialog = useAppSelector(
        selectNearbyDialog.bind(null, episode, time, count)
    );

    return (
        <>
            {dialog.map((d) => (
                <Dialog key={d} episode={episode} time={d}></Dialog>
            ))}
        </>
    );
};

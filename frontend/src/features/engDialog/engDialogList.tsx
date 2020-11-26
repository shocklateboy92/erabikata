import { useAppSelector } from 'app/hooks';
import React from 'react';
import { FC, useEffect } from 'react';
import { useDispatch } from 'react-redux';
import { EngDialog } from './engDialog';
import './slice';
import { fetchEnglishDialog, selectNearbyEnglishDialog } from './slice';

export interface IEngDialogProps {
    time: number;
    episodeId: string;
}
export const EngDialogList: FC<IEngDialogProps> = ({ episodeId, time }) => {
    const dispatch = useDispatch();
    useEffect(() => {
        dispatch(fetchEnglishDialog([episodeId, time]));
    }, [dispatch, episodeId, time]);

    const dialog = useAppSelector((state) =>
        selectNearbyEnglishDialog(state, episodeId, time)
    );

    return (
        <>
            {dialog.map((dialogTime) => (
                <EngDialog
                    key={dialogTime}
                    episodeId={episodeId}
                    time={dialogTime}
                />
            ))}
        </>
    );
};

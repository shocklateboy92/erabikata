import React, { FC } from 'react';
import { EngDialog } from './engDialog';
import './slice';
import { useEngSubsIndexQuery } from 'backend';
import { QueryPlaceholder } from '../../components/placeholder/queryPlaceholder';

export interface IEngDialogProps {
    time: number;
    episodeId: string;
}

export const EngDialogList: FC<IEngDialogProps> = ({ episodeId, time }) => {
    const response = useEngSubsIndexQuery({ episodeId, time, count: 3 });
    if (!response.data) {
        return <QueryPlaceholder result={response} />;
    }

    return (
        <>
            {response.data.dialog.map((content) => (
                <EngDialog key={content.id} content={content} />
            ))}
        </>
    );
};

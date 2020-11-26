import { FC } from 'react';
import React from 'react';

export const EpisodeTitle: FC<{ id: string }> = ({ id }) => {
    return <p>{id}</p>;
};

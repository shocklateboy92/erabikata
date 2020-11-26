import { mdiShare } from '@mdi/js';
import Icon from '@mdi/react';
import { IconProps } from '@mdi/react/dist/IconProps';
import React, { FC } from 'react';
import { Link } from 'react-router-dom';

interface IWordLinkParams {
    word: string;
    includeEpisode: string;
    includeTime: number;
    iconSize: IconProps['size'];
}

export const WordLink: FC<IWordLinkParams> = ({
    word,
    includeEpisode,
    includeTime,
    iconSize
}) => {
    const search = new URLSearchParams({
        includeEpisode,
        includeTime: includeTime.toString()
    }).toString();

    return (
        <Link
            to={{
                pathname: '/word/' + word,
                search: search
            }}
        >
            <Icon path={mdiShare} size={iconSize} />
        </Link>
    );
};

import { mdiShare } from '@mdi/js';
import Icon from '@mdi/react';
import { IconProps } from '@mdi/react/dist/IconProps';
import React, { FC } from 'react';
import { Link } from 'react-router-dom';

interface IWordLinkParams {
    word: string;
    includeEpisode?: string;
    includeTime?: number;
    iconSize: IconProps['size'];
}

export const WordLink: FC<IWordLinkParams> = ({
    word,
    includeEpisode,
    includeTime,
    iconSize
}) => {
    const search =
        includeEpisode && includeTime
            ? new URLSearchParams({
                  includeEpisode,
                  includeTime: includeTime.toString()
              }).toString()
            : undefined;

    return (
        <Link
            to={{
                pathname: '/ui/word/' + word,
                search: search
            }}
        >
            <Icon path={mdiShare} size={iconSize} />
        </Link>
    );
};

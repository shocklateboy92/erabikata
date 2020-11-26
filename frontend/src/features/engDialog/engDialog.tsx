import { useAppSelector } from 'app/hooks';
import { InlineError } from 'components/inlineError';
import React from 'react';
import { FC } from 'react';
import reactStringReplace from 'react-string-replace';
import { IEngDialogProps } from './engDialogList';
import { selectEnglishDialogContent } from './slice';

export const EngDialog: FC<IEngDialogProps> = ({ episodeId, time }) => {
    const content = useAppSelector((state) =>
        selectEnglishDialogContent(state, episodeId, time)
    );

    if (!content) {
        return (
            <InlineError>
                Trying to display an english dialog that has not been fetched
            </InlineError>
        );
    }

    return (
        <p>
            {reactStringReplace(content.text, /(\\n)/gi, () => (
                <br />
            ))}
        </p>
    );
};

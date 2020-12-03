import { useAppSelector } from 'app/hooks';
import { InlineError } from 'components/inlineError';
import React, { FC } from 'react';
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
            {content.text?.map((text, index) => (
                <>
                    {text}
                    {index > 0 && <br />}
                </>
            ))}
        </p>
    );
};

import { useAppSelector } from 'app/hooks';
import { InlineError } from 'components/inlineError';
import React, { FC, Fragment } from 'react';
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
                <Fragment key={index}>
                    {text}
                    {index > 0 && <br />}
                </Fragment>
            ))}
        </p>
    );
};

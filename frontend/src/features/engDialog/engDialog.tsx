import { useAppSelector, useTypedSelector } from 'app/hooks';
import classNames from 'classnames';
import { InlineError } from 'components/inlineError';
import { formatTime } from 'components/time';
import {
    dialogSelection,
    selectIsCurrentlySelected
} from 'features/selectedWord';
import React, { FC, Fragment } from 'react';
import { useDispatch } from 'react-redux';
import { IEngDialogProps } from './engDialogList';
import { selectEnglishDialogContent } from './slice';

export const EngDialog: FC<IEngDialogProps> = ({ episodeId, time }) => {
    const content = useAppSelector((state) =>
        selectEnglishDialogContent(state, episodeId, time)
    );
    const highlightColor = useTypedSelector((state) =>
        selectIsCurrentlySelected(state, episodeId, time)
    );
    const dispatch = useDispatch();

    if (!content) {
        return (
            <InlineError>
                Trying to display an english dialog that has not been fetched
            </InlineError>
        );
    }

    return (
        <p
            className={classNames({ highlightColor })}
            onClick={() => {
                dispatch(dialogSelection({ time: content.time }));
            }}
        >
            {formatTime(content.time)}
            <br />
            {content.text?.map((text, index) => (
                <Fragment key={index}>
                    {index > 0 && <br />}
                    {text}
                </Fragment>
            ))}
        </p>
    );
};

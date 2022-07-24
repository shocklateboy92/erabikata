import { useTypedSelector } from 'app/hooks';
import { useAppDispatch } from 'app/store';
import { Sentence } from 'backend.generated';
import classNames from 'classnames';
import { formatTime } from 'components/time';
import {
    dialogSelection,
    selectIsCurrentlySelected
} from 'features/selectedWord';
import { FC, Fragment } from 'react';

export const EngDialog: FC<{ content: Sentence; compact?: boolean }> = ({
    content: { episodeId, text, time },
    compact
}) => {
    const highlightColor = useTypedSelector((state) =>
        selectIsCurrentlySelected(state, episodeId, time)
    );
    const dispatch = useAppDispatch();

    return (
        <p
            className={classNames({ highlightColor })}
            onClick={() => {
                dispatch(dialogSelection({ time: time, episode: episodeId }));
            }}
        >
            {!compact && (
                <>
                    {formatTime(time)}
                    <br />
                </>
            )}
            {text.map((text, index) => (
                <Fragment key={index}>
                    {index > 0 && <br />}
                    {text}
                </Fragment>
            ))}
        </p>
    );
};

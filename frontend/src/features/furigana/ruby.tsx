import { useTypedSelector } from 'app/hooks';
import classNames from 'classnames';
import React, { FC } from 'react';
import { isKana } from './kana';
import {
    selectIsFuriganaEnabled,
    selectIsFuriganaHiddenForWords
} from './slice';
import './ruby.scss';

interface IRubyProps extends React.ComponentProps<'ruby'> {
    reading?: string;
    wordIds: number[];
    active?: boolean;
    known?: boolean;
}

export const Ruby: FC<IRubyProps> = ({
    children,
    reading,
    wordIds,
    known,
    className,
    active,
    ...rest
}) => {
    const hideReading = useTypedSelector(
        (state) =>
            selectIsFuriganaHiddenForWords(state, wordIds) ||
            !selectIsFuriganaEnabled(state)
    );

    return (
        <ruby className={classNames({ active, known }, className)} {...rest}>
            {children ?? reading}
            {!hideReading &&
                children &&
                !isKana(children) &&
                reading !== '*' && <rt>{reading}</rt>}
        </ruby>
    );
};

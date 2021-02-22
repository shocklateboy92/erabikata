import { useTypedSelector } from 'app/hooks';
import React, { FC } from 'react';
import { isKana } from './kana';
import {
    selectIsFuriganaEnabled,
    selectIsFuriganaHiddenForWords
} from './slice';

interface IRubyProps extends React.ComponentProps<'ruby'> {
    reading?: string;
    wordIds: number[];
}

export const Ruby: FC<IRubyProps> = ({
    children,
    reading,
    wordIds,
    ...rest
}) => {
    const hideReading = useTypedSelector(
        (state) =>
            selectIsFuriganaHiddenForWords(state, wordIds) ||
            !selectIsFuriganaEnabled(state)
    );

    return (
        <ruby {...rest}>
            {children ?? reading}
            {!hideReading &&
                children &&
                !isKana(children) &&
                reading !== '*' && <rt>{reading}</rt>}
        </ruby>
    );
};

import { useTypedSelector } from 'app/hooks';
import React, { FC } from 'react';
import { isKana } from './kana';
import { selectIsFuriganaHiddenForWord } from './slice';

interface IRubyProps extends React.ComponentProps<'ruby'> {
    reading?: string;
    baseForm: string;
}

export const Ruby: FC<IRubyProps> = ({
    children,
    reading,
    baseForm,
    ...rest
}) => {
    const hideReading = useTypedSelector((state) =>
        selectIsFuriganaHiddenForWord(state, baseForm)
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

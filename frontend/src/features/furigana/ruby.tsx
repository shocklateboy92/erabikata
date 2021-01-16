import { useTypedSelector } from 'app/hooks';
import React, { FC } from 'react';
import { isKana } from './kana';

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
    const hideReading = useTypedSelector(
        (state) => state.furigana.words[baseForm]?.hide
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

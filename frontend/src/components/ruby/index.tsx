import React, { FC } from 'react';

interface IRubyProps extends React.ComponentProps<'ruby'> {
    reading?: string;
    hideReading?: boolean;
}

export const Ruby: FC<IRubyProps> = ({
    children,
    reading,
    hideReading,
    ...rest
}) => (
    <ruby {...rest}>
        {children ?? reading}
        {!hideReading &&
            children &&
            children !== reading &&
            reading !== '*' && <rt>{reading}</rt>}
    </ruby>
);

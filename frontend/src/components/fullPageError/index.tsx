import { FullWidthText } from 'components/fullWidth';
import React, { FC } from 'react';

export const FullPageError: FC = (props) => (
    <FullWidthText>Error: {props.children}</FullWidthText>
);

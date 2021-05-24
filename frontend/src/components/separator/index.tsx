import classNames from 'classnames';
import React, { FC } from 'react';
import './separator.scss';

export const Separator: FC<{ navBar?: boolean }> = ({ navBar }) => (
    <div
        className={classNames('separator', {
            navBar
        })}
    />
);

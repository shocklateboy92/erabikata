import classNames from 'classnames';
import { FC } from 'react';
import './layout.scss';

export const Row: FC<{ centerChildren?: boolean }> = ({
    centerChildren,
    children
}) => (
    <div className={classNames('layout row', { centerChildren })}>
        {children}
    </div>
);

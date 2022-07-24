import classNames from 'classnames';
import { FC, PropsWithChildren } from 'react';
import './layout.scss';

export const Row: FC<PropsWithChildren<{ centerChildren?: boolean }>> = ({
    centerChildren,
    children
}) => (
    <div className={classNames('layout row', { centerChildren })}>
        {children}
    </div>
);

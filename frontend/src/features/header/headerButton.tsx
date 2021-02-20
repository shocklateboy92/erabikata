import classNames from 'classnames';
import React, { FC } from 'react';
import { HeaderItem } from './headerItem';

export interface IToolButtonProps {
    icon: string;
    active?: boolean;
    onClick: React.ComponentProps<'button'>['onClick'];
}

export const HeaderButton: FC<IToolButtonProps> = ({ active, ...props }) => (
    <button
        className={classNames({ active })}
        onClickCapture={props.onClick}
        onMouseDown={(e) => e.preventDefault()}
    >
        <HeaderItem icon={props.icon} children={props.children} />
    </button>
);

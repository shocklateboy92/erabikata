import classNames from 'classnames';
import React, { FC } from 'react';
import { HeaderItem } from './headerItem';

export interface IToolButtonProps {
    icon: string;
    active?: boolean;
    hideOnMobile?: boolean;
    onClick: React.ComponentProps<'button'>['onClick'];
}

export const HeaderButton: FC<IToolButtonProps> = ({
    active,
    hideOnMobile,
    ...props
}) => (
    <button
        className={classNames({ active, hideOnMobile })}
        onClickCapture={props.onClick}
        onMouseDown={(e) => e.preventDefault()}
    >
        <HeaderItem icon={props.icon} children={props.children} />
    </button>
);

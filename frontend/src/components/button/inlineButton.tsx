import classNames from 'classnames';
import React, { FC } from 'react';
import styles from './button.module.scss';

export interface IButtonProps {
    onClick?: React.ComponentProps<'button'>['onClick'];
    className?: string;
    complex?: boolean;
}
export const InlineButton: FC<IButtonProps> = (props) => {
    return (
        <button
            className={classNames(
                styles.inline,
                { [styles.complex]: props.complex },
                props.className
            )}
            onClickCapture={props.onClick}
            onMouseDown={(e) => e.preventDefault()}
        >
            {props.children}
        </button>
    );
};

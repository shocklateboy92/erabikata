import classNames from 'classnames';
import React, { FC } from 'react';
import styles from './button.module.scss';

export interface IButtonProps {
    onClick?: React.ComponentProps<'button'>['onClick'];
    className?: string;
    complex?: boolean;
    small?: boolean;
    hideOnMobile?: boolean;
}
export const InlineButton: FC<IButtonProps> = (props) => {
    return (
        <button
            className={classNames(
                styles.inline,
                {
                    [styles.complex]: props.complex,
                    [styles.small]: props.small,
                    hideOnMobile: props.hideOnMobile
                },
                props.className
            )}
            onClickCapture={props.onClick}
            onMouseDown={(e) => e.preventDefault()}
        >
            {props.children}
        </button>
    );
};

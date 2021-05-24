import Icon from '@mdi/react';
import React from 'react';
import { FC } from 'react';
import { IButtonProps, InlineButton } from './inlineButton';
import styles from './button.module.scss';
import classNames from 'classnames';

export const ActionButton: FC<
    IButtonProps & { icon: string; spinIcon?: boolean }
> = ({ icon, spinIcon, children, className, ...rest }) => (
    <InlineButton className={classNames([styles.action], className)} {...rest}>
        <Icon
            className={classNames({ animateSelfSpinFast: spinIcon })}
            path={icon}
        />
        {children}
    </InlineButton>
);

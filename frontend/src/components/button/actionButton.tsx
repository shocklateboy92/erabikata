import Icon from '@mdi/react';
import React from 'react';
import { FC } from 'react';
import { IButtonProps, InlineButton } from './inlineButton';
import styles from './button.module.scss';
import classNames from 'classnames';
import { mdiLoading } from '@mdi/js';

export const ActionButton: FC<
    IButtonProps & { icon: string; isLoading?: boolean }
> = ({ icon, isLoading, children, className, ...rest }) => (
    <InlineButton className={classNames([styles.action], className)} {...rest}>
        <Icon
            className={classNames('icon', { animateSelfSpinFast: isLoading })}
            path={isLoading ? mdiLoading : icon}
        />
        <div>{children}</div>
    </InlineButton>
);

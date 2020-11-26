import classNames from 'classnames';
import React, { FC } from 'react';
import styles from './separator.module.scss';

export const Separator: FC<{ navBar?: boolean }> = (props) => (
    <div
        className={classNames(styles.separator, {
            [styles.navBar]: props.navBar
        })}
    />
);
